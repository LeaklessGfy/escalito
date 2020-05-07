namespace Cocktails
{
    public class Order
    {
        private Order(Cocktail cocktail)
        {
            Cocktail = cocktail;
        }

        public Cocktail Cocktail { get; }

        public static Order BuildRandom()
        {
            return new Order(CocktailBuilder.BuildRandom());
        }
    }
}