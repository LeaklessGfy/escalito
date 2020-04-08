namespace Core
{
    class Order
    {
        public Character Client { get; }
        public Cocktail Cocktail { get; }
        public Order(Character client, Cocktail cocktail)
        {
            Client = client;
            Cocktail = cocktail;
        }
    }
}
