namespace SnackVendingMachine.Hardware;

public class ConsoleDisplay : IDisplay
{
	public void Show(string message)
	{
		Console.WriteLine(message);
	}
}
