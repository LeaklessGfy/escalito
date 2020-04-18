using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Core
{
    public class Cocktail
    {
        private Cocktail(CocktailKey key, int price, IReadOnlyDictionary<IngredientKey, int> recipe)
        {
            Key = key;
            Price = price;
            Recipe = recipe;
        }

        public CocktailKey Key { get; }
        public int Price { get; }
        public IReadOnlyDictionary<IngredientKey, int> Recipe { get; }

        private static Cocktail Build(CocktailKey key)
        {
            var price = BuildPrice(key);
            var recipe = BuildRecipe(key);
            return new Cocktail(key, price, recipe);
        }

        public static Cocktail BuildRandom()
        {
            var names = Enum.GetNames(typeof(CocktailKey)).Where(e => !e.Equals(CocktailKey.Custom.ToString()))
                .ToArray();
            var rand = Random.Range(0, names.Length);
            Enum.TryParse(names[rand], out CocktailKey name);
            return Build(name);
        }

        public static Cocktail BuildCustom(IReadOnlyDictionary<IngredientKey, int> recipe)
        {
            return new Cocktail(CocktailKey.Custom, 0, recipe);
        }

        private static int BuildPrice(CocktailKey key)
        {
            switch (key)
            {
                case CocktailKey.Mojito:
                    return 5;
                case CocktailKey.CubaLibre:
                    return 10;
                case CocktailKey.Rum:
                    return 5;
                case CocktailKey.Coca:
                    return 3;
                case CocktailKey.Lemonade:
                    return 3;
                case CocktailKey.Custom:
                    return 0;
                default:
                    return 0;
            }
        }

        private static IReadOnlyDictionary<IngredientKey, int> BuildRecipe(CocktailKey key)
        {
            var recipe = new Dictionary<IngredientKey, int>();

            switch (key)
            {
                case CocktailKey.Mojito:
                    recipe.Add(IngredientKey.Rum, 100);
                    break;
                case CocktailKey.CubaLibre:
                    recipe.Add(IngredientKey.Rum, 50);
                    recipe.Add(IngredientKey.Cola, 50);
                    break;
                case CocktailKey.Rum:
                    recipe.Add(IngredientKey.Rum, 100);
                    break;
                case CocktailKey.Coca:
                    recipe.Add(IngredientKey.Cola, 100);
                    break;
                case CocktailKey.Lemonade:
                    recipe.Add(IngredientKey.Lemonade, 100);
                    break;
                case CocktailKey.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }

            return recipe;
        }
    }
}