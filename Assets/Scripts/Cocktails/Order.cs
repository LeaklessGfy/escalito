namespace Cocktails
{
    public class Order
    {
        private Order(Cocktail cocktail)
        {
            Cocktail = cocktail;
        }

        public Cocktail Cocktail { get; }

        public decimal Price => Cocktail.Price;

        public static Order BuildRandom()
        {
            return new Order(CocktailBuilder.BuildRandom());
        }
    }
}