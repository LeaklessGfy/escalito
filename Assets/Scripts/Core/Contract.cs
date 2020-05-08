using Bonuses;

namespace Core
{
    public class Contract
    {
        public int Price { get; }
        public Expense Expense { get; }
        public IBonus Bonus { get; }
    }
}