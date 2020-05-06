using Core;
using Singleton;

namespace Components
{
    public class PayableIngredient : Payable
    {
        private IngredientKey _ingredient;

        public IngredientKey IngredientKey
        {
            set
            {
                _ingredient = value;
                SetPrice(CashManager.GetPrice(value));
                SetName(_ingredient.ToString());
            }
        }

        protected override void Buy()
        {
            if (Price > CashManager.Main.Cash)
            {
                return;
            }
            CashManager.Main.Cash -= Price;
            Controller.Main.IngredientsExpense.Add(new Expense()
            {
                Title = _ingredient.ToString(),
                Amount = Price // TODO : Probably not buying price
            });
        }
    }
}