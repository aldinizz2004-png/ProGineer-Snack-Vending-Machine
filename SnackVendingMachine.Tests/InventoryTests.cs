using SnackVendingMachine.Exceptions;
using SnackVendingMachine.Models;
using InventoryModel = SnackVendingMachine.Inventory.Inventory;

namespace SnackVendingMachine.Tests;

public class InventoryTests
{
	[Fact]
	public void AddSlot_DuplicateCode_ThrowsVendingException()
	{
		InventoryModel inventory = new InventoryModel();
		inventory.AddSlot(11);

		Assert.Throws<VendingException>(() => inventory.AddSlot(11));
	}

	[Fact]
	public void GetSlot_InvalidCode_ThrowsVendingException()
	{
		InventoryModel inventory = new InventoryModel();

		Assert.Throws<VendingException>(() => inventory.GetSlot(99));
	}

	[Fact]
	public void Restock_ValidQuantity_AddsRequestedStock()
	{
		InventoryModel inventory = new InventoryModel();
		inventory.AddSlot(11);
		Snack snack = new Snack("Chips", 150);

		inventory.Restock(11, snack, 3);

		Assert.Equal(3, inventory.GetSlot(11).Stock);
	}
}
