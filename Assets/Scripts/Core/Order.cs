using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Order : IEnumerable<Cocktail>
    {
        private readonly HashSet<Cocktail> _pendingCocktails;
        private readonly HashSet<Cocktail> _servedCocktails;

        public Order(Cocktail[] cocktails)
        {
            All = new List<Cocktail>(cocktails);
            _pendingCocktails = new HashSet<Cocktail>(cocktails);
            _servedCocktails = new HashSet<Cocktail>();
        }

        public IReadOnlyList<Cocktail> All { get; }
        public bool Ready => _pendingCocktails.Count == 0;
        public int ExpectedPrice => _servedCocktails.Sum(c => c.Price);
        public int Count => All.Count;

        public IEnumerator<Cocktail> GetEnumerator()
        {
            return _pendingCocktails.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pendingCocktails.GetEnumerator();
        }

        public (Cocktail, int) FindBest(Customer customer, Cocktail actual)
        {
            var def = (_pendingCocktails.First(), 0);

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