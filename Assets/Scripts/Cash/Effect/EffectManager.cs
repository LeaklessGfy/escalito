using System.Collections.Generic;
using System.Linq;
using Characters;

namespace Cash.Effect
{
    public class EffectManager
    {
        private readonly HashSet<IEffect> _effects = new HashSet<IEffect>();

        public void Add(IEffect effect)
        {
            _effects.Add(effect);
        }

        public decimal Apply(Customer customer, decimal amount)
        {
            return _effects.Aggregate(amount, (current, effect) => effect.Apply(customer, current));
        }
    }
}