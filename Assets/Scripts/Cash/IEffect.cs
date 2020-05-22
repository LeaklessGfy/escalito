using Characters.Impl;

namespace Cash
{
    public interface IEffect
    {
        decimal Amount { get; }
        string Details { get; }
        decimal Apply(Customer customer, decimal amount);
    }
}