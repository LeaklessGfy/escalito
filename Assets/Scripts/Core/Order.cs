using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Core
{
    public class Order
    {
        private readonly Queue<Cocktail> _pendingCocktails;
        private readonly HashSet<Cocktail> _servedCocktails;

        private Order(Cocktail[] cocktails)
        {
            All = new List<Cocktail>(cocktails);
            _pendingCocktails = new Queue<Cocktail>(cocktails);
            _servedCocktails = new HashSet<Cocktail>();
        }

        public IReadOnlyList<Cocktail> All { get; }
        public IReadOnlyCollection<Cocktail> Pending => _pendingCocktails;
        public bool Ready => _pendingCocktails.Count == 0;
        public int ExpectedPrice => _servedCocktails.Sum(c => c.Price);
        public int Count => All.Count;

        public Cocktail Next()
        {
            if (_pendingCocktails.Count < 1)
            {
                throw new InvalidOperationException("No more pending order");
            }

            var expected = _pendingCocktails.Dequeue();
            _servedCocktails.Add(expected);

            return expected;
        }

        public static Order BuildRandom()
        {
            var size = Random.Range(1, 4);
            var arr = new Cocktail[size];

            for (var i = 0; i < size; i++) arr[i] = Cocktail.BuildRandom();

            return new Order(arr);
        }
    }
}