using Characters;

namespace Bonuses
{
    public interface IBonus
    {
        int Apply(Customer customer, int current);
    }
}