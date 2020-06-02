using System.Collections.Generic;
using Ingredients;

namespace Cocktails
{
    public class Cocktail
    {
        internal Cocktail(CocktailKey key, decimal price, Dictionary<IngredientKey, int> recipe)
        {
            Key = key;
            Price = price;
            Recipe = recipe;
        }

        public CocktailKey Key { get; }
        public decimal Price { get; }
        public Dictionary<IngredientKey, int> Recipe { get; }

        public void AddIngredient(IngredientKey key)
        {
            Recipe.TryGetValue(key, out var prev);
            Recipe[key] = prev + 1;
        }
    }
}