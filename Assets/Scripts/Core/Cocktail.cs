using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Cocktail
    {
        public Recipe Recipe
        {
            get; private set;
        }
        public IReadOnlyDictionary<Consumable, int> Formula
        {
            get; private set;
        }

        private Cocktail(Recipe recipe, IReadOnlyDictionary<Consumable, int> formula)
        {
            Recipe = recipe;
            Formula = formula;
        }

        private static Cocktail Build(Recipe recipe)
        {
            var dict = BuildFormula(recipe);
            return new Cocktail(recipe, dict);
        }

        public static Cocktail BuildRandom()
        {
            var types = Enum.GetNames(typeof(Recipe)).Where(e => !e.Equals(Recipe.Custom.ToString())).ToArray();
            var rand = UnityEngine.Random.Range(0, types.Length);
            Enum.TryParse(types[rand], out Recipe type);
            return Build(type);
        }

        public static Cocktail BuildCustom(Dictionary<Consumable, int> formula)
        {
            return new Cocktail(Recipe.Custom, formula);
        }

        private static IReadOnlyDictionary<Consumable, int> BuildFormula(Recipe type)
        {
            var dict = new Dictionary<Consumable, int>();
            switch (type)
            {
                case Recipe.Mojito:
                    dict.Add(Consumable.Rum, 100);
                    return dict;
                case Recipe.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            throw new ArgumentException("No formula for specific type " + type);
        }
    }
}
