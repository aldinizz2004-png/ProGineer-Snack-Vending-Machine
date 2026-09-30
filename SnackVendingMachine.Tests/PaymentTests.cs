using SnackVendingMachine.Exceptions;
using PaymentModel = SnackVendingMachine.Payment.Payment;

namespace SnackVendingMachine.Tests;

public class PaymentTests
{
	[Theory]
	[InlineData("Crypto", 1.00)]
	[InlineData("Coin", 0.25)]
	[InlineData("Note", 10.00)]
	[InlineData("Coin", 0.00)]
	[InlineData("Coin", -1.00)]
	public void Validate_InvalidPayment_ThrowsVendingException(string type, double amount)
	{
		PaymentModel payment = new PaymentModel(type, (decimal)amount);

		Assert.Throws<VendingException>(() => payment.Validate());
	}
}
