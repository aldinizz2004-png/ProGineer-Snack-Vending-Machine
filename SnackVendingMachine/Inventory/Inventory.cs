using SnackVendingMachine.Exceptions;
using SnackVendingMachine.Models;

namespace SnackVendingMachine.Inventory;

public class Inventory
{
	private Dictionary<int, Slot> _slots;

	public Inventory()
	{
		_slots = new Dictionary<int, Slot>();
	}

	public void AddSlot(int code)
	{
		if (_slots.ContainsKey(code))
		{
			throw new VendingException("Slot code already exists.");
		}

		Slot slot = new Slot(code);
		_slots.Add(code, slot);
	}

	public Slot GetSlot(int code)
	{
		if (!_slots.ContainsKey(code))
		{
			throw new VendingException("Invalid slot code.");
		}

		return _slots[code];
	}

	public void Restock(int code, Snack snack, int quantity)
	{
		if (quantity <= 0)
		{
			throw new VendingException("Restock quantity must be greater than zero.");
		}

		Slot slot = GetSlot(code);

		for (int i = 0; i < quantity; i++)
		{
			slot.AddSnack(snack);
		}
	}
}
