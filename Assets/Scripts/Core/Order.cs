using System.Collections.Generic;

namespace Core
{
    public class Order
    {
        public Queue<Cocktail> Cocktails { get; } = new Queue<Cocktail>();
    }
}