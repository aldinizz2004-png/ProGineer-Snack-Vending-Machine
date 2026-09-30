namespace SnackVendingMachine.Exceptions;

public class VendingException : Exception
{
	public VendingException(string message)
		: base(message)
	{
	}
}