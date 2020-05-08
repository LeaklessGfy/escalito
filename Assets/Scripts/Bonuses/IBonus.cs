using Characters;

namespace Bonuses
{
    public interface IBonus
    {
        int Apply(int amount, int satisfaction);
    }
}