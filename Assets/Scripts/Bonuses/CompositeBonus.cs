using System;

namespace Bonuses
{
    public class CompositeBonus : IBonus
    {
        public event Func<int, int, int> Bonus;

        public void AddBonus(IBonus bonus)
        {
            Bonus += bonus.Apply;
        }

        public int Apply(int amount, int satisfaction)
        {
            return Bonus?.Invoke(amount, satisfaction) ?? amount;
        }
    }
}