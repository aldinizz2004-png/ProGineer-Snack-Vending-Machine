using SnackVendingMachine.Inventory;
using SnackVendingMachine.Machine;
using SnackVendingMachine.Models;
using SnackVendingMachine.Payment;
using SnackVendingMachine.Exceptions;
using SnackVendingMachine.Hardware;

Inventory inventory = new Inventory();
CashVault cashVault = new CashVault();
ConsoleDisplay display = new ConsoleDisplay();
ConsoleDispenser dispenser = new ConsoleDispenser();

inventory.AddSlot(11);
inventory.AddSlot(12);
inventory.AddSlot(13);

Snack chips = new Snack("Chips", 150);
Snack chocolate = new Snack("Chocolate", 200);
Snack water = new Snack("Water", 100);

inventory.Restock(11, chips, 5);
inventory.Restock(12, chocolate, 5);
inventory.Restock(13, water, 5);

cashVault.Deposit(0.10m, 20);
cashVault.Deposit(0.20m, 20);
cashVault.Deposit(0.50m, 20);
cashVault.Deposit(1.00m, 20);

SnackMachine machine = new SnackMachine(inventory, cashVault, display, dispenser);

void InsertAndDisplay(Payment payment)
{
	machine.InsertPayment(payment);
	Console.WriteLine($"Inserted: ${payment.Amount:0.00}");
	Console.WriteLine($"Current balance: ${machine.CurrentBalance:0.00}");
}

while (true)
{
	Console.Clear();

	Console.WriteLine("==============================");
	Console.WriteLine("      SNACK VENDING MACHINE   ");
	Console.WriteLine("==============================");
	Console.WriteLine();

	Console.WriteLine("11 - Chips       $1.50");
	Console.WriteLine("12 - Chocolate   $2.00");
	Console.WriteLine("13 - Water       $1.00");

	Console.WriteLine();
	Console.WriteLine("Enter slot code or 0 to exit:");

	string? input = Console.ReadLine();

	if (!int.TryParse(input, out int code))
	{
		Console.WriteLine("Invalid input.");
		Console.ReadKey();
		continue;
	}

	if (code == 0)
	{
		break;
	}

	try
	{
		machine.SelectItem(code);
		Slot selectedSlot = inventory.GetSlot(code);
		decimal selectedPrice = selectedSlot.PeekSnack().PriceCents / 100m;

		Console.WriteLine();
		Console.WriteLine($"Selected item price: ${selectedPrice:0.00}");
		Console.WriteLine();
		Console.WriteLine("Insert payment:");
		Console.WriteLine("1 - $0.10");
		Console.WriteLine("2 - $0.20");
		Console.WriteLine("3 - $0.50");
		Console.WriteLine("4 - $1.00");
		Console.WriteLine("5 - $20.00");
		Console.WriteLine("6 - $50.00");
		Console.WriteLine("9 - Complete Purchase");
		Console.WriteLine("0 - Cancel");

		bool paymentFinished = false;

		while (!paymentFinished)
		{
			Console.Write("Choice: ");
			string? paymentInput = Console.ReadLine();

			try
			{
				switch (paymentInput)
				{
					case "1":
						InsertAndDisplay(new Payment("Coin", 0.10m));
						break;

					case "2":
						InsertAndDisplay(new Payment("Coin", 0.20m));
						break;

					case "3":
						InsertAndDisplay(new Payment("Coin", 0.50m));
						break;

					case "4":
						InsertAndDisplay(new Payment("Coin", 1.00m));
						break;

					case "5":
						InsertAndDisplay(new Payment("Note", 20.00m));
						break;

					case "6":
						InsertAndDisplay(new Payment("Note", 50.00m));
						break;

					case "9":
						decimal change = machine.CurrentBalance - selectedPrice;
						machine.CompleteSale();
						Console.WriteLine();
						Console.WriteLine("Purchase completed successfully.");
						Console.WriteLine($"Change: ${change:0.00}");
						Console.WriteLine($"Remaining stock: {selectedSlot.Stock}");
						paymentFinished = true;
						break;

					case "0":
						machine.Cancel();
						Console.WriteLine("Purchase cancelled.");
						paymentFinished = true;
						break;

					default:
						Console.WriteLine("Invalid choice.");
						break;
				}
			}
			catch (VendingException ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
		}
	}
	catch (VendingException ex)
	{
		Console.WriteLine($"Error: {ex.Message}");
	}

	Console.WriteLine();
	Console.WriteLine("Press any key to continue...");
	Console.ReadKey();
}
