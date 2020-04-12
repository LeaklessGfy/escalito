using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Core
{
    public class Cocktail
    {
        private Cocktail(CocktailName name, int price, IReadOnlyDictionary<Ingredient, int> recipe)
        {
            Name = name;
            Price = price;
            Recipe = recipe;
        }

        public CocktailName Name { get; }
        public int Price { get; }
        public IReadOnlyDictionary<Ingredient, int> Recipe { get; }

        private static Cocktail Build(CocktailName name)
        {
            var price = BuildPrice(name);
            var recipe = BuildRecipe(name);
            return new Cocktail(name, price, recipe);
        }

        public static Cocktail BuildRandom()
        {
            var names = Enum.GetNames(typeof(CocktailName)).Where(e => !e.Equals(CocktailName.Custom.ToString()))
                .ToArray();
            var rand = Random.Range(0, names.Length);
            Enum.TryParse(names[rand], out CocktailName name);
            return Build(name);
        }

        public static Cocktail BuildCustom(IReadOnlyDictionary<Ingredient, int> recipe)
        {
            return new Cocktail(CocktailName.Custom, 0, recipe);
        }

        private static int BuildPrice(CocktailName name)
        {
            switch (name)
            {
                case CocktailName.Mojito:
                    return 5;
                case CocktailName.CubaLibre:
                    return 10;
                case CocktailName.Rum:
                    return 5;
                case CocktailName.Coca:
                    return 3;
                case CocktailName.Lemonade:
                    return 3;
                case CocktailName.Custom:
                    return 0;
                default:
                    return 0;
            }
        }

        private static IReadOnlyDictionary<Ingredient, int> BuildRecipe(CocktailName name)
        {
            var recipe = new Dictionary<Ingredient, int>();

            switch (name)
            {
                case CocktailName.Mojito:
                    recipe.Add(Ingredient.Rum, 100);
                    break;
                case CocktailName.CubaLibre:
                    recipe.Add(Ingredient.Rum, 50);
                    recipe.Add(Ingredient.Cola, 50);
                    break;
                case CocktailName.Rum:
                    recipe.Add(Ingredient.Rum, 100);
                    break;
                case CocktailName.Coca:
                    recipe.Add(Ingredient.Cola, 100);
                    break;
                case CocktailName.Lemonade:
                    recipe.Add(Ingredient.Lemonade, 100);
                    break;
                case CocktailName.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(name), name, null);
            }

            return recipe;
        }
    }
}