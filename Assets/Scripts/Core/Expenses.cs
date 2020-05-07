using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Expenses
    {
        private readonly Dictionary<ExpenseKey, List<Expense>> _expenses = new Dictionary<ExpenseKey, List<Expense>>();

        public void AddExpense(Expense expense)
        {
            if (_expenses.TryGetValue(expense.Type, out var list))
            {
                list.Add(expense);
            }
            else
            {
                _expenses.Add(expense.Type, new List<Expense>() { expense });
            }
        }

        public Dictionary<ExpenseKey, int> Sum()
        {
            return _expenses
                .Select(expense => (expense.Key, expense.Value.Sum(subExpense => subExpense.Amount)))
                .ToDictionary(x => x.Key, x => x.Item2);
        }
    }
}