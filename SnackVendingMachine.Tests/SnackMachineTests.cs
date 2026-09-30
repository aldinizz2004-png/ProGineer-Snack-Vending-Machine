using SnackVendingMachine.Exceptions;
using SnackVendingMachine.Hardware;
using SnackVendingMachine.Machine;
using SnackVendingMachine.Models;
using SnackVendingMachine.Payment;
using InventoryModel = SnackVendingMachine.Inventory.Inventory;
using PaymentModel = SnackVendingMachine.Payment.Payment;

namespace SnackVendingMachine.Tests;

public class SnackMachineTests
{
	[Fact]
	public void CompleteSale_ExactPayment_DispensesOnceAndResetsTransaction()
	{
		(SnackMachine machine, Slot slot) = CreateMachine(100, 2);
		machine.SelectItem(11);
		machine.InsertPayment(new PaymentModel("Coin", 1.00m));

		machine.CompleteSale();

		Assert.Equal(1, slot.Stock);
		Assert.Equal(0m, machine.CurrentBalance);
		machine.SelectItem(11);
		machine.Cancel();
	}

	[Fact]
	public void CompleteSale_Overpayment_DispensesAndReturnsChange()
	{
		(SnackMachine machine, Slot slot) = CreateMachine(150);
		machine.SelectItem(11);
		machine.InsertPayment(new PaymentModel("Coin", 1.00m));
		machine.InsertPayment(new PaymentModel("Coin", 1.00m));

		machine.CompleteSale();

		Assert.Equal(0, slot.Stock);
		Assert.Equal(0m, machine.CurrentBalance);
	}

	[Fact]
	public void CompleteSale_InsufficientFunds_ThrowsWithoutDispensing()
	{
		(SnackMachine machine, Slot slot) = CreateMachine(150);
		machine.SelectItem(11);
		machine.InsertPayment(new PaymentModel("Coin", 1.00m));

		Assert.Throws<VendingException>(() => machine.CompleteSale());
		Assert.Equal(1, slot.Stock);
		Assert.Equal(1.00m, machine.CurrentBalance);
	}

	[Fact]
	public void CompleteSale_ExactChangeUnavailable_ThrowsWithoutDispensing()
	{
		(SnackMachine machine, Slot slot) = CreateMachine(155);
		machine.SelectItem(11);
		machine.InsertPayment(new PaymentModel("Coin", 1.00m));
		machine.InsertPayment(new PaymentModel("Coin", 1.00m));

		Assert.Throws<VendingException>(() => machine.CompleteSale());
		Assert.Equal(1, slot.Stock);
		Assert.Equal(2.00m, machine.CurrentBalance);
	}

	[Fact]
	public void SelectItem_OutOfStock_ThrowsVendingException()
	{
		(SnackMachine machine, _) = CreateMachine(100, 0);

		Assert.Throws<VendingException>(() => machine.SelectItem(11));
	}

	[Fact]
	public void Cancel_ActiveCashTransaction_RefundsAndResetsTransaction()
	{
		(SnackMachine machine, Slot slot) = CreateMachine(100);
		machine.SelectItem(11);
		machine.InsertPayment(new PaymentModel("Coin", 1.00m));

		machine.Cancel();

		Assert.Equal(0m, machine.CurrentBalance);
		Assert.Equal(1, slot.Stock);
		machine.SelectItem(11);
		machine.Cancel();
	}

	[Fact]
	public void InsertPayment_NoSelectedItem_ThrowsVendingException()
	{
		(SnackMachine machine, _) = CreateMachine(100);

		Assert.Throws<VendingException>(
			() => machine.InsertPayment(new PaymentModel("Coin", 1.00m)));
	}

	[Fact]
	public void SelectItem_PaymentAlreadyInProgress_ThrowsVendingException()
	{
		InventoryModel inventory = new InventoryModel();
		inventory.AddSlot(11);
		inventory.AddSlot(12);
		inventory.Restock(11, new Snack("Chips", 100), 1);
		inventory.Restock(12, new Snack("Water", 100), 1);
		SnackMachine machine = new SnackMachine(
			inventory,
			new CashVault(),
			new FakeDisplay(),
			new FakeDispenser());
		machine.SelectItem(11);

		Assert.Throws<VendingException>(() => machine.SelectItem(12));
	}

	[Fact]
	public void CompleteSale_NoSelectedItem_ThrowsVendingException()
	{
		(SnackMachine machine, _) = CreateMachine(100);

		Assert.Throws<VendingException>(() => machine.CompleteSale());
	}

	[Fact]
	public void CompleteSale_ExactCardPayment_CompletesPurchase()
	{
		(SnackMachine machine, Slot slot) = CreateMachine(150);
		machine.SelectItem(11);
		machine.InsertPayment(new PaymentModel("Card", 1.50m));

		machine.CompleteSale();

		Assert.Equal(0, slot.Stock);
		Assert.Equal(0m, machine.CurrentBalance);
	}

	[Fact]
	public void InsertPayment_InexactCardPayment_ThrowsWithoutChangingBalance()
	{
		(SnackMachine machine, Slot slot) = CreateMachine(150);
		machine.SelectItem(11);

		Assert.Throws<VendingException>(
			() => machine.InsertPayment(new PaymentModel("Card", 2.00m)));
		Assert.Equal(0m, machine.CurrentBalance);
		Assert.Equal(1, slot.Stock);
	}

	[Fact]
	public void CompleteSale_SuccessfulSale_CallsDispenserExactlyOnce()
	{
		FakeDispenser dispenser = new FakeDispenser();
		(SnackMachine machine, _) = CreateMachine(100, dispenser: dispenser);
		machine.SelectItem(11);
		machine.InsertPayment(new PaymentModel("Coin", 1.00m));

		machine.CompleteSale();

		Assert.Equal(1, dispenser.CallCount);
		Assert.Equal(11, dispenser.LastSlotCode);
	}

	[Fact]
	public void CompleteSale_InsufficientFunds_DoesNotCallDispenser()
	{
		FakeDispenser dispenser = new FakeDispenser();
		(SnackMachine machine, _) = CreateMachine(150, dispenser: dispenser);
		machine.SelectItem(11);
		machine.InsertPayment(new PaymentModel("Coin", 1.00m));

		Assert.Throws<VendingException>(() => machine.CompleteSale());
		Assert.Equal(0, dispenser.CallCount);
	}

	private static (SnackMachine Machine, Slot Slot) CreateMachine(
		long priceCents,
		int quantity = 1,
		IDispenser? dispenser = null)
	{
		InventoryModel inventory = new InventoryModel();
		inventory.AddSlot(11);

		if (quantity > 0)
		{
			inventory.Restock(11, new Snack("Test snack", priceCents), quantity);
		}

		Slot slot = inventory.GetSlot(11);
		SnackMachine machine = new SnackMachine(
			inventory,
			new CashVault(),
			new FakeDisplay(),
			dispenser ?? new FakeDispenser());
		return (machine, slot);
	}

	private class FakeDisplay : IDisplay
	{
		public void Show(string message)
		{
		}
	}

	private class FakeDispenser : IDispenser
	{
		public int CallCount { get; private set; }
		public int? LastSlotCode { get; private set; }

		public void Dispense(int slotCode)
		{
			CallCount++;
			LastSlotCode = slotCode;
		}
	}
}
