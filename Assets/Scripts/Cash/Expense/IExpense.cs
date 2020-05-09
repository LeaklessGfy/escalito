namespace Cash.Expense
{
    public interface IExpense
    {
        ExpenseKey Type { get; }
        decimal Amount { get; }
        string Details { get; }
    }
}