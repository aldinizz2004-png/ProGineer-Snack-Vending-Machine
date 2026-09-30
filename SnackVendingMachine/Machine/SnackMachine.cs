using SnackVendingMachine.Exceptions;
using SnackVendingMachine.Hardware;
using SnackVendingMachine.Inventory;
using SnackVendingMachine.Payment;
using SnackVendingMachine.States;
using SnackVendingMachine.Models;

namespace SnackVendingMachine.Machine;

public class SnackMachine
{
	private Inventory.Inventory _inventory;
	private CashVault _cashVault;
	private IDisplay _display;
	private IDispenser _dispenser;
	private IMachineState _state;

	private int? _selectedCode;
	private decimal _balance;
	private decimal _cashBalance;
	public decimal CurrentBalance => _balance;

	public SnackMachine(
		Inventory.Inventory inventory,
		CashVault cashVault,
		IDisplay display,
		IDispenser dispenser)
	{
		_inventory = inventory;
		_cashVault = cashVault;
		_display = display;
		_dispenser = dispenser;
		_state = new IdleState();

		_selectedCode = null;
		_balance = 0;
		_cashBalance = 0;
	}

	public void SelectItem(int code)
	{
		_state.SelectItem(this, code);
	}

	public void InsertPayment(Payment.Payment payment)
	{
		_state.InsertPayment(this, payment);
	}

	public void Cancel()
	{
		_state.Cancel(this);
	}

	public void SetSelectedItem(int code)
	{
		Slot slot = _inventory.GetSlot(code);

		if (slot.Stock == 0)
		{
			throw new VendingException("Item is out of stock.");
		}

		_selectedCode = code;
		_state = new PaymentState();
		_display.Show($"Item selected from slot {code}.");
	}

	public void AddPayment(Payment.Payment payment)
	{
		payment.Validate();

		if (payment.Type == "Card")
		{
			if (_selectedCode == null)
			{
				throw new VendingException("No item selected.");
			}

			Snack selectedSnack = _inventory.GetSlot(_selectedCode.Value).PeekSnack();
			decimal price = selectedSnack.PriceCents / 100m;
			decimal remainingAmount = price - _balance;

			if (payment.Amount != remainingAmount)
			{
				throw new VendingException("Card payment must exactly settle the transaction.");
			}

			_balance += payment.Amount;
			_display.Show($"Payment accepted: ${payment.Amount:0.00}.");
			return;
		}

		if (payment.Type == "Coin" || payment.Type == "Note")
		{
			_cashVault.Deposit(payment.Amount, 1);
			_cashBalance += payment.Amount;
		}

		_balance += payment.Amount;
		_display.Show($"Payment accepted: ${payment.Amount:0.00}.");
	}

	public void CompleteSale()
	{
		if (_selectedCode == null)
		{
			throw new VendingException("No item selected.");
		}

		Slot slot = _inventory.GetSlot(_selectedCode.Value);

		if (slot.Stock == 0)
		{
			throw new VendingException("Item is out of stock.");
		}

		Snack snack = slot.PeekSnack();
		decimal price = snack.PriceCents / 100m;

		if (_balance < price)
		{
			throw new VendingException("Insufficient funds.");
		}

		decimal change = _balance - price;

		if (change > 0 && !_cashVault.CanMakeChange(change))
		{
			throw new VendingException("Exact change is not available.");
		}

		_state = new DispensingState();

		slot.DispenseOne();
		_dispenser.Dispense(_selectedCode.Value);

		if (change > 0)
		{
			_cashVault.MakeChange(change);
		}

		_balance = 0;
		_cashBalance = 0;
		_selectedCode = null;
		_state = new IdleState();
		_display.Show("Purchase completed.");
	}

	public void CancelCurrentPurchase()
	{
		decimal refund = _cashBalance;

		if (_cashBalance > 0)
		{
			_cashVault.MakeChange(_cashBalance);
		}

		_balance = 0;
		_cashBalance = 0;
		_selectedCode = null;
		_state = new IdleState();

		if (refund > 0)
		{
			_display.Show($"Purchase cancelled. Refunded: ${refund:0.00}.");
		}
		else
		{
			_display.Show("Purchase cancelled.");
		}
	}
}
