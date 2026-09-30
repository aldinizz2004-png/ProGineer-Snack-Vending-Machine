using SnackVendingMachine.Machine;
using SnackVendingMachine.Exceptions;
using PaymentModel = SnackVendingMachine.Payment.Payment;

namespace SnackVendingMachine.States;

public class IdleState : IMachineState
{
	public void SelectItem(SnackMachine machine, int code)
	{
		machine.SetSelectedItem(code);
	}

	public void InsertPayment(SnackMachine machine, PaymentModel payment)
	{
		throw new VendingException("Select an item first.");
	}

	public void Cancel(SnackMachine machine)
	{
		throw new VendingException("No active purchase.");
	}
}
