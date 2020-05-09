using Cash;
using Cash.Expense;
using Ingredients;

namespace Components
{
    public class IngredientItem : Item
    {
        private IngredientKey _ingredient;

        public IngredientKey IngredientKey
        {
            get => _ingredient;
            set
            {
                _ingredient = value;
                SetPrice(CashController.GetPrice(value));
                SetName(_ingredient.ToString());
            }
        }

        protected override void Buy()
        {
            if (Price > CashController.Main.Cash)
            {
                return;
            }

            CashController.Main.Pay(Price);
            MainController.Main.Ingredients.Add(_ingredient, true);
            CashController.Main.ExpenseManager.Add(new IngredientExpense(_ingredient, Price));
        }

        protected override bool ForbidBuy()
        {
            return MainController.Main.Ingredients.ContainsKey(_ingredient);
        }
    }
}