namespace Core
{
    internal class Order
    {
        public Order(Character client, Cocktail cocktail)
        {
            Client = client;
            Cocktail = cocktail;
        }

        public Character Client { get; }
        public Cocktail Cocktail { get; }
    }
}