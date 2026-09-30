using SnackVendingMachine.Exceptions;

namespace SnackVendingMachine.Payment;

public class CashVault
{
    	private SortedDictionary<decimal, int> _stock;

        public CashVault()
        {
            _stock = new SortedDictionary<decimal, int>();
            // Initialize the cash vault with some denominations and their counts
            _stock[0.10m] = 10; // 10 coins of 10 cents
            _stock[0.20m] = 10; // 10 coins of 20 cents
            _stock[0.50m] = 10; // 10 coins of 50 cents
            _stock[1.00m] = 10; // 10 coins of 1 dollar
            _stock[20.00m] = 5; // 5 notes of 20 dollars
            _stock[50.00m] = 5; // 5 notes of 50 dollars
        }

        public void Deposit(decimal denomination, int count)  // Deposit cash into the vault
        {
           if (!_stock.ContainsKey(denomination))
		    {
			    throw new VendingException("Invalid denomination.");
		    }

		    if (count <= 0)
		    {
			    throw new VendingException("Count must be greater than zero.");
		    }

		        _stock[denomination] += count;
	    }

        public bool CanMakeChange(decimal amount)
        {
            return TryBuildChange(amount, out _);
        }

        public Dictionary<decimal, int> MakeChange(decimal amount)
        {
            if (!TryBuildChange(amount, out Dictionary<decimal, int> change))
            {
                throw new VendingException("Exact change is not available.");
            }

            foreach (KeyValuePair<decimal, int> item in change)
            {
                _stock[item.Key] -= item.Value;
            }

            return change;
        }

        private bool TryBuildChange(decimal amount, out Dictionary<decimal, int> change)
        {
            Dictionary<decimal, int> result = new Dictionary<decimal, int>();

            if (amount < 0)
            {
                change = result;
                return false;
            }

            List<decimal> denominations = new List<decimal>(_stock.Keys);
            denominations.Reverse();

            bool FindChange(int index, decimal remaining)
            {
                if (remaining == 0)
                {
                    return true;
                }

                if (index == denominations.Count)
                {
                    return false;
                }

                decimal denomination = denominations[index];
                int maximumCount = Math.Min(_stock[denomination], (int)(remaining / denomination));

                for (int count = maximumCount; count >= 0; count--)
                {
                    decimal nextRemaining = remaining - (denomination * count);

                    if (FindChange(index + 1, nextRemaining))
                    {
                        if (count > 0)
                        {
                            result[denomination] = count;
                        }

                        return true;
                    }
                }

                return false;
            }

            bool found = FindChange(0, amount);
            change = result;
            return found;
        }
        
}
