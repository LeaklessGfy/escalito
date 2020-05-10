using System.Collections.Generic;
using System.Linq;

namespace Cash.Expense
{
    public class ExpenseManager
    {
        private readonly Dictionary<ExpenseKey, List<IExpense>>
            _expenses = new Dictionary<ExpenseKey, List<IExpense>>();

        public bool HasExpense()
        {
            return _expenses.Count > 0;
        }

        public void Add(IExpense expense)
        {
            if (_expenses.TryGetValue(expense.Type, out var list))
            {
                list.Add(expense);
            }
            else
            {
                _expenses.Add(expense.Type, new List<IExpense> {expense});
            }
        }

        public IReadOnlyDictionary<ExpenseKey, decimal> Sum()
        {
            return _expenses
                .Select(expense => (expense.Key, expense.Value.Sum(subExpense => subExpense.Amount)))
                .ToDictionary(x => x.Key, x => x.Item2);
        }
    }
}