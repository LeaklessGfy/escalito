using Characters.Impl;

namespace Cash.Effect
{
    public class ComboBonus : IEffect
    {
        private readonly MainController _mainController;

        public ComboBonus(MainController mainController, decimal amount)
        {
            _mainController = mainController;
            Amount = amount;
        }

        public decimal Amount { get; }
        public string Details { get; }

        public decimal Apply(Customer customer, decimal amount)
        {
            return amount * _mainController.PositiveCombo * Amount;
        }
    }
}