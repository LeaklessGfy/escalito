using System;
using System.Collections.Generic;
using System.Linq;
using Singleton;
using Random = UnityEngine.Random;

namespace Core
{
    public class Cocktail
    {
        private Cocktail(CocktailKey key, int price, Dictionary<IngredientKey, int> recipe)
        {
            Key = key;
            Price = price;
            Recipe = recipe;
        }

        public CocktailKey Key { get; }
        public int Price { get; }
        public Dictionary<IngredientKey, int> Recipe { get; }

        public void AddIngredient(IngredientKey key)
        {
            Recipe.TryGetValue(key, out var prev);
            Recipe[key] = prev + 1;
        }

        private static Cocktail Build(CocktailKey key)
        {
            var recipe = BuildRecipe(key);
            var price = CashManager.Main.GetPrice(key);
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

        public static Cocktail BuildCustom(Dictionary<IngredientKey, int> recipe)
        {
            return new Cocktail(CocktailKey.Custom, 0, recipe);
        }

        public static Cocktail BuildEmpty()
        {
            return new Cocktail(CocktailKey.Custom, 0, new Dictionary<IngredientKey, int>());
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