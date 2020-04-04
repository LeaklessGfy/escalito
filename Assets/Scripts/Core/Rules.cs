using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Core
{
    class Rules
    {
        public static float BaseRule(Cocktail expected, Cocktail actual)
        {
            float sum = 0;
            float total = 0;

            foreach (KeyValuePair<Cocktail.Consumable, int> consumable in expected.Formula)
            {
                actual.Formula.TryGetValue(consumable.Key, out int actualValue);

                float difference = Math.Abs(consumable.Value - actualValue);
                float sub = actualValue - consumable.Value;
                float div = sub / consumable.Value;
                float percent = div * 100;

                sum += percent;
                total++;
            }

            foreach (KeyValuePair<Cocktail.Consumable, int> consumable in actual.Formula.Where(d => !expected.Formula.ContainsKey(d.Key)))
            {
                sum += 0;
                total++;
            }

            return sum / total;
        }
    }
}
