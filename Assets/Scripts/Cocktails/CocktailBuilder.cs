using System;
using System.Collections.Generic;
using System.Linq;
using Ingredients;
using Random = UnityEngine.Random;

namespace Cocktails
{
    public static class CocktailBuilder
    {
        public static Cocktail BuildRandom()
        {
            var keys = Enum.GetValues(typeof(CocktailKey))
                .Cast<CocktailKey>()
                .Where(k => !k.Equals(CocktailKey.Custom))
                .ToArray();
            var rand = Random.Range(0, keys.Length);
            var key = keys[rand];

            return Build(key);
        }

        public static Cocktail BuildEmpty()
        {
            return Build(CocktailKey.Custom);
        }

        private static Cocktail Build(CocktailKey key)
        {
            var recipe = BuildRecipe(key);
            var price = CashController.GetPrice(key);
            return new Cocktail(key, price, recipe);
        }

        private static Dictionary<IngredientKey, int> BuildRecipe(CocktailKey key)
        {
            var recipe = new Dictionary<IngredientKey, int>();

            switch (key)
            {
                case CocktailKey.Mojito:
                    recipe.Add(IngredientKey.Rum, 100);
                    recipe.Add(IngredientKey.Lemon, 1);
                    break;
                case CocktailKey.CubaLibre:
                    recipe.Add(IngredientKey.Rum, 50);
                    recipe.Add(IngredientKey.Cola, 50);
                    recipe.Add(IngredientKey.Lemon, 1);
                    break;
                case CocktailKey.Rum:
                    recipe.Add(IngredientKey.Rum, 100);
                    break;
                case CocktailKey.Cola:
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