using System;
using System.Linq;

namespace Core
{
    static class Rules
    {
        public static float BaseRule(Cocktail expected, Cocktail actual)
        {
            float sum = 0;
            float total = 0;

            foreach (var consumable in expected.Formula)
            {
                actual.Formula.TryGetValue(consumable.Key, out int actualValue);

                float difference = Math.Abs(consumable.Value - actualValue);
                float sub = actualValue - consumable.Value;
                var div = sub / consumable.Value;
                var percent = div * 100;

                sum += percent;
                total++;
            }

            foreach (var consumable in actual.Formula.Where(d => !expected.Formula.ContainsKey(d.Key)))
            {
                sum += 0;
                total++;
            }

            return sum / total;
        }
    }
}
