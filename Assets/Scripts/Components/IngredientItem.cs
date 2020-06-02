using Cash;
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
            if (Price > MagicBag.Bag.cash.Cash)
            {
                return;
            }

            MagicBag.Bag.cash.Cash -= Price;
            MagicBag.Bag.main.Ingredients.Add(_ingredient, true);
            // CashController.Main.ExpenseManager.Add(new IngredientExpense(_ingredient, Price));
        }

        protected override bool ForbidBuy()
        {
            return MagicBag.Bag.main.Ingredients.ContainsKey(_ingredient);
        }
    }
}