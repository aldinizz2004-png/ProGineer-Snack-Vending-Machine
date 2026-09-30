using SnackVendingMachine.Machine;
using PaymentModel = SnackVendingMachine.Payment.Payment;

namespace SnackVendingMachine.States;

public interface IMachineState
{
	void SelectItem(SnackMachine machine, int code);

	void InsertPayment(SnackMachine machine, PaymentModel payment);

	void Cancel(SnackMachine machine);
}