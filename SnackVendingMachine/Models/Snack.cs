// Snack.cs

namespace SnackVendingMachine.Models
{
    public class Snack
    {
        public string Name { get; private set; }
        public long PriceCents { get; private set; }

        public Snack(string name, long priceCents) // Constructor
        {
            Name = name;
            PriceCents = priceCents;
        }
    }
}
