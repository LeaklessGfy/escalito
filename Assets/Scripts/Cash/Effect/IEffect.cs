using Characters;

namespace Cash.Effect
{
    public interface IEffect
    {
        decimal Amount { get; }
        string Details { get; }
        decimal Apply(Customer customer, decimal amount);
    }
}