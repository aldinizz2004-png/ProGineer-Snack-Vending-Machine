using SnackVendingMachine.Exceptions;

namespace SnackVendingMachine.Models;

public class Slot
{
	public int Code { get; private set; }
	private Stack<Snack> _snacks;

	public int Stock
	{
		get
		{
			return _snacks.Count;
		}
	}

	public Slot(int code)
	{
		Code = code;
		_snacks = new Stack<Snack>();
	}

	public void AddSnack(Snack snack)
	{
		_snacks.Push(snack);
	}

	public Snack DispenseOne()
	{
		if (_snacks.Count == 0)
		{
			throw new VendingException("Item is out of stock.");
		}

		Snack snack = _snacks.Pop();
		return snack;
	}
	public Snack PeekSnack()
	{
		if (_snacks.Count == 0)
		{
			throw new VendingException("Item is out of stock.");
		}

		return _snacks.Peek();
	}
}
