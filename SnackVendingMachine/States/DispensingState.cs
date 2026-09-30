using SnackVendingMachine.Machine;
using SnackVendingMachine.Exceptions;
using PaymentModel = SnackVendingMachine.Payment.Payment;

namespace SnackVendingMachine.States;

public class DispensingState : IMachineState
{
	public void SelectItem(SnackMachine machine, int code)
	{
		throw new VendingException("Machine is dispensing.");
	}

	public void InsertPayment(SnackMachine machine, PaymentModel payment)
	{
		throw new VendingException("Machine is dispensing.");
	}

	public void Cancel(SnackMachine machine)
	{
		throw new VendingException("Purchase cannot be cancelled while dispensing.");
	}
}
