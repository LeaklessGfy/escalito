using Characters;
using UnityEngine;

namespace Core
{
    public class Bonus
    {
        public static int SatisfactionBonus(Customer customer, int amount)
        {
            if (customer.Satisfaction < PercentHelper.High)
            {
                return amount;
            }

            var bonus = Random.Range(0, 4) == 0;
            return amount + (bonus ? Random.Range(5, 10) : 0);
        }

        public static int ComboBonus(Customer customer, int amount)
        {
            return amount;
        }
    }
}