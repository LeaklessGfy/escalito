namespace Cash.Expense
{
    public class ContractExpense : IExpense
    {
        public ContractExpense(decimal amount)
        {
            Amount = amount;
            Details = "";
        }

        public ExpenseKey Type => ExpenseKey.Contracts;
        public decimal Amount { get; }
        public string Details { get; }
    }
}