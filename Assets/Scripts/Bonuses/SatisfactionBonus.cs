using Characters;
using Core;
using UnityEngine;

namespace Bonuses
{
    public class SatisfactionBonus : IBonus
    {
        public int Apply(Customer customer, int current)
        {
            if (customer.Satisfaction < PercentHelper.High)
            {
                return current;
            }

            var bonus = Random.Range(0, 4) == 0;
            return current + (bonus ? Random.Range(5, 10) : 0);
        }
    }
}