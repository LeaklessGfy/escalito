using Core;
using UnityEngine;

namespace Bonuses
{
    public class SatisfactionBonus : IBonus
    {
        public int Apply(int amount, int satisfaction)
        {
            if (satisfaction < PercentHelper.High)
            {
                return amount;
            }

            var bonus = Random.Range(0, 4) == 0;
            return amount + (bonus ? Random.Range(5, 10) : 0);
        }
    }
}