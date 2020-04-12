using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    internal static class Rules
    {
        public static float BaseRule(Cocktail expected, Cocktail actual)
        {
            float sum = 0;
            float total = 0;

            foreach (var consumable in expected.Recipe)
            {
                actual.Recipe.TryGetValue(consumable.Key, out var actualValue);

                float difference = Math.Abs(consumable.Value - actualValue);
                float sub = actualValue - consumable.Value;
                var div = sub / consumable.Value;
                var percent = div * 100;

                sum += percent;
                total++;
            }

            foreach (var consumable in actual.Recipe.Where(d => !expected.Recipe.ContainsKey(d.Key)))
            {
                sum += 0;
                total++;
            }

            return sum / total;
        }

        public static float CocktailRule(Cocktail expected, Cocktail actual)
        {
            var unitSatisfaction = new List<float>();

            foreach (var ingredient in expected.Recipe)
            {
                actual.Recipe.TryGetValue(ingredient.Key, out var actualValue);
                var satisfaction = ComputeSatisfaction(ingredient.Value, actualValue);
                unitSatisfaction.Add(satisfaction);
            }

            var total = unitSatisfaction.Sum() / unitSatisfaction.Count;
            return total;
        }

        private static float ComputeSatisfaction(float expectedValue, float actualValue)
        {
            var difference = actualValue - expectedValue;
            var differencePercentage = difference / expectedValue * 100;

            return 100 + differencePercentage;
        }
    }
}