using Core;
using Singleton;

namespace Components
{
    public class PayableIngredient : Payable
    {
        private IngredientKey _ingredient;

        public IngredientKey IngredientKey
        {
            get => _ingredient;
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
            Controller.Main.Ingredients.Add(_ingredient, true);
            Controller.Main.Expenses.AddExpense(new Expense(ExpenseKey.Ingredients, Price)
            {
                Details = _ingredient.ToString()
            });
        }

        protected override bool ForbidBuy()
        {
            return Controller.Main.Ingredients.ContainsKey(_ingredient);
        }
    }
}