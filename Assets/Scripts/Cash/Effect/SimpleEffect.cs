using System;
using Characters.Impl;

namespace Cash.Effect
{
    public class SimpleEffect : IEffect
    {
        private readonly Func<Customer, decimal, decimal> _apply;

        public SimpleEffect(decimal amount, Func<Customer, decimal, decimal> apply)
        {
            Amount = amount;
            _apply = apply;
        }

        public decimal Amount { get; }
        public string Details { get; }

        decimal IEffect.Apply(Customer customer, decimal amount)
        {
            return _apply(customer, amount);
        }
    }
}