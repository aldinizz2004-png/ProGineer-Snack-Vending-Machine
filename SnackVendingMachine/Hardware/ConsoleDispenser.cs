namespace SnackVendingMachine.Hardware;

public class ConsoleDispenser : IDispenser
{
	public void Dispense(int slotCode)
	{
		Console.WriteLine($"Dispensing item from slot {slotCode}");
	}
}
