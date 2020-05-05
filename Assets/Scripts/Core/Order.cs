namespace Core
{
    public class Order
    {
        public Cocktail Cocktail { get; }

        private Order(Cocktail cocktail)
        {
            Cocktail = cocktail;
        }

        public static Order BuildRandom()
        {
            return new Order(Cocktail.BuildRandom());
        }
    }
}