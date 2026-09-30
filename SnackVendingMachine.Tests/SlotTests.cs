using SnackVendingMachine.Models;

using SnackVendingMachine.Exceptions;

namespace SnackVendingMachine.Tests;

public class SlotTests
{
	[Fact]
	public void AddSnack_ValidSnack_IncreasesStock()
	{
		Slot slot = new Slot(11);
		Snack snack = new Snack("Chips", 150);

		slot.AddSnack(snack);

		Assert.Equal(1, slot.Stock);
	}

	[Fact]
	public void DispenseOne_MultipleSnacks_ReturnsLastAddedSnack()
	{
		Slot slot = new Slot(11);

		Snack firstSnack = new Snack("Chips", 150);
		Snack secondSnack = new Snack("Chocolate", 200);

		slot.AddSnack(firstSnack);
		slot.AddSnack(secondSnack);

		Snack result = slot.DispenseOne();

		Assert.Same(secondSnack, result);
		Assert.Equal(1, slot.Stock);
	}

	[Fact]
	public void PeekSnack_PopulatedSlot_ReturnsTopWithoutChangingStock()
	{
		Slot slot = new Slot(11);
		Snack snack = new Snack("Chips", 150);
		slot.AddSnack(snack);

		Snack result = slot.PeekSnack();

		Assert.Same(snack, result);
		Assert.Equal(1, slot.Stock);
	}

	[Fact]
	public void DispenseOne_EmptySlot_ThrowsVendingException()
	{
		Slot slot = new Slot(11);

		Assert.Throws<VendingException>(() => slot.DispenseOne());
	}

	[Fact]
	public void PeekSnack_EmptySlot_ThrowsVendingException()
	{
		Slot slot = new Slot(11);

		Assert.Throws<VendingException>(() => slot.PeekSnack());
	}
}
