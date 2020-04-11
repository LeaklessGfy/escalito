using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Cocktail
    {
        public CocktailName Name { get; }
        public int Price { get; }
        public IReadOnlyDictionary<Consumable, int> Recipe { get; }

        private Cocktail(CocktailName name, int price, IReadOnlyDictionary<Consumable, int> recipe)
        {
            Name = name;
            Price = price;
            Recipe = recipe;
        }

        private static Cocktail Build(CocktailName name)
        {
            var price = BuildPrice(name);
            var recipe = BuildRecipe(name);
            return new Cocktail(name, price, recipe);
        }

        public static Cocktail BuildRandom()
        {
            var names = Enum.GetNames(typeof(CocktailName)).Where(e => !e.Equals(CocktailName.Custom.ToString())).ToArray();
            var rand = UnityEngine.Random.Range(0, names.Length);
            Enum.TryParse(names[rand], out CocktailName name);
            return Build(name);
        }

        public static Cocktail BuildCustom(IReadOnlyDictionary<Consumable, int> recipe)
        {
            return new Cocktail(CocktailName.Custom, 0, recipe);
        }

        private static int BuildPrice(CocktailName name)
        {
            switch (name)
            {
                case CocktailName.Mojito:
                    return 5;
                case CocktailName.Custom:
                    return 0;
                default:
                    return 0;
            }
        }

        private static IReadOnlyDictionary<Consumable, int> BuildRecipe(CocktailName name)
        {
            var recipe = new Dictionary<Consumable, int>();
            switch (name)
            {
                case CocktailName.Mojito:
                    recipe.Add(Consumable.Rum, 100);
                    return recipe;
                case CocktailName.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(name), name, null);
            }
            throw new ArgumentException("No formula for specific type " + name);
        }
    }
}
