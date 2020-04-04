using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Core
{
    public class Cocktail
    {
        public enum CocktailType
        {
            Mojito,
            Custom
        };
        public enum Consumable
        {
            Rhum
        };

        public CocktailType Type
        {
            get; private set;
        }
        public IReadOnlyDictionary<Consumable, int> Formula
        {
            get; private set;
        }

        private Cocktail(CocktailType type, IReadOnlyDictionary<Consumable, int> formula)
        {
            Type = type;
            Formula = formula;
        }

        public static Cocktail Build(CocktailType type)
        {
            IReadOnlyDictionary<Consumable, int> dict = BuildFormula(type);
            return new Cocktail(type, dict);
        }

        public static Cocktail BuildRandom()
        {
            string[] types = Enum.GetNames(typeof(CocktailType)).Where(e => !e.Equals(CocktailType.Custom.ToString())).ToArray();
            int rand = UnityEngine.Random.Range(0, types.Length);
            Enum.TryParse(types[rand], out CocktailType type);
            return Build(type);
        }

        public static Cocktail BuildCustom(Dictionary<Consumable, int> formula)
        {
            return new Cocktail(CocktailType.Custom, formula);
        }

        private static IReadOnlyDictionary<Consumable, int> BuildFormula(CocktailType type)
        {
            Dictionary<Consumable, int> dict = new Dictionary<Consumable, int>();
            switch (type)
            {
                case CocktailType.Mojito:
                    dict.Add(Consumable.Rhum, 100);
                    return dict;
            }
            throw new ArgumentException("No formula for specific type " + type);
        }
    }
}
