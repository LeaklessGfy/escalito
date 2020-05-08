using Core;

namespace Bonuses
{
    public class ComboBonus : IBonus
    {
        public int Apply(int amount, int satisfaction)
        {
            if (satisfaction <= PercentHelper.Low)
            {
                return amount;
            }

            return amount;
        }
    }
}