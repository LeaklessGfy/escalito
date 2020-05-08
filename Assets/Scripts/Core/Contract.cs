using System;
using Characters;

namespace Core
{
    public class Contract
    {
        public int Price { get; }
        public Expense Expense { get; }
        public Func<Customer, int, int> Bonus { get; }
        public Func<Customer, int, int> Penalty { get; }
    }
}