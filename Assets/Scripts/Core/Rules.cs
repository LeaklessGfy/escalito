using System.Collections.Generic;
using System.Linq;

namespace Core
{
    internal static class Rules
    {
        public static float CocktailRule(Cocktail expected, Cocktail actual)
        {
            var satisfactions = new List<float>();

            foreach (var ingredient in expected.Recipe)
            {
                actual.Recipe.TryGetValue(ingredient.Key, out var actualValue);
                var satisfaction = ComputeSatisfaction(ingredient.Value, actualValue);
                satisfactions.Add(satisfaction);
            }

            foreach (var ingredient in actual.Recipe)
            {
                if (!expected.Recipe.ContainsKey(ingredient.Key))
                {
                    satisfactions.Add(0);
                }
            }

            var total = satisfactions.Sum() / satisfactions.Count;
            return total;
        }

        private static float ComputeSatisfaction(float expectedValue, float actualValue)
        {
            var difference = actualValue - expectedValue;
            var differencePercentage = difference / expectedValue * 100;

            if (differencePercentage > 10)
            {
                return 110;
            }

            return 100 + differencePercentage;
        }
    }
}