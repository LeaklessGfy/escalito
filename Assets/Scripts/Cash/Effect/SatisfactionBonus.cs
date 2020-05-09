using Characters;
using Core;
using UnityEngine;

namespace Cash.Effect
{
    public class SatisfactionBonus : IEffect
    {
        public decimal Amount { get; }
        public string Details { get; }

        public decimal Apply(Customer customer, decimal amount)
        {
            if (customer.Satisfaction < PercentHelper.High)
            {
                return amount;
            }

            var bonus = Random.Range(0, 4) == 0;
            return amount + (bonus ? Random.Range(5, 10) : 0);
        }
    }
}