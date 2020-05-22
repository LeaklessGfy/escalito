using Cash.Trigger;

namespace Cash.Contract
{
    public class Contract
    {
        internal Contract(decimal price, CashTrigger cashTrigger, IEffect bonus, IEffect penalty)
        {
            Price = price;
            CashTrigger = cashTrigger;
            Bonus = bonus;
            Penalty = penalty;
        }

        public decimal Price { get; }
        public CashTrigger CashTrigger { get; }
        public IEffect Bonus { get; }
        public IEffect Penalty { get; }
    }
}