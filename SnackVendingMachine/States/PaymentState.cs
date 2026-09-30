using SnackVendingMachine.Machine;
using SnackVendingMachine.Exceptions;
using PaymentModel = SnackVendingMachine.Payment.Payment;

namespace SnackVendingMachine.States;

public class PaymentState : IMachineState
{
	public void SelectItem(SnackMachine machine, int code)
	{
		throw new VendingException("Item already selected.");
	}

	public void InsertPayment(SnackMachine machine, PaymentModel payment)
	{
		machine.AddPayment(payment);
	}

	public void Cancel(SnackMachine machine)
	{
		machine.CancelCurrentPurchase();
	}
}
