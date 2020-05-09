using Ingredients;

namespace Cash.Expense
{
    public class IngredientExpense : IExpense
    {
        public IngredientExpense(IngredientKey ingredient, decimal amount)
        {
            Amount = amount;
            Details = ingredient.ToString();
        }

        public ExpenseKey Type => ExpenseKey.Ingredients;
        public decimal Amount { get; }
        public string Details { get; }
    }
}