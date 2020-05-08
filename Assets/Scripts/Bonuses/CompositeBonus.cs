using System;
using Characters;

namespace Bonuses
{
    public class CompositeBonus : IBonus
    {
        public event Func<Customer, int, int> Bonus;

        public void Add(IBonus bonus)
        {
            Bonus += bonus.Apply;
        }

        public int Apply(Customer customer, int current)
        {
            return Bonus?.Invoke(customer, current) ?? current;
        }
    }
}