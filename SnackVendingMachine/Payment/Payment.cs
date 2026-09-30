
using SnackVendingMachine.Exceptions;

namespace SnackVendingMachine.Payment;
public class Payment
{
	public string Type { get; private set; }
	public decimal Amount { get; private set; }
	public Payment(string type, decimal amount)     // Constructor
	{
		Type = type;
		Amount = amount;
	}

	public void Validate()
	{
		if (Amount <= 0)
		{
			throw new VendingException("Payment amount must be greater than zero.");
		}

		if (Type != "Coin" && Type != "Note" && Type != "Card")
		{
			throw new VendingException("Invalid payment type.");
		}

		if (Type == "Coin")
		{
			if (Amount != 0.10m &&
				Amount != 0.20m &&
				Amount != 0.50m &&
				Amount != 1.00m)
			{
				throw new VendingException("Invalid coin denomination.");
			}
		}

		if (Type == "Note")
		{
			if (Amount != 20.00m && Amount != 50.00m)
			{
				throw new VendingException("Invalid note denomination.");
			}
		}
	}
}
