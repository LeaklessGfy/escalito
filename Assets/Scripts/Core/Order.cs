using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Order
    {
        private readonly HashSet<Cocktail> _pendingCocktails;
        private readonly HashSet<Cocktail> _servedCocktails;

        public bool Ready => _pendingCocktails.Count == 0;
        public int ExpectedPrice => _servedCocktails.Sum(c => c.Price);
        public int Count => _pendingCocktails.Count + _servedCocktails.Count;

        public Order(IEnumerable<Cocktail> cocktails)
        {
            _pendingCocktails = new HashSet<Cocktail>(cocktails);
            _servedCocktails = new HashSet<Cocktail>();
        }

        public (Cocktail, int) FindBest(Customer customer, Cocktail actual)
        {
            (Cocktail, int) def = (_pendingCocktails.First(), 0);

            foreach (var expected in _pendingCocktails)
            {
                var satisfaction = customer.Try(expected, actual);

                if (satisfaction <= def.Item2)
                {
                    continue;
                }

                def.Item1 = expected;
                def.Item2 = satisfaction;
            }

            _pendingCocktails.Remove(def.Item1);
            _servedCocktails.Add(def.Item1);

            return def;
        }
    }
}