using Characters;

namespace Bonuses
{
    public class ComboBonus : IBonus
    {
        public int Apply(Customer customer, int current)
        {
            if (!customer.Satisfied)
            {
                return current;
            }

            return current;
        }
    }
}