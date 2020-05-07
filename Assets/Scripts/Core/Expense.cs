namespace Core
{
    public class Expense
    {
        public ExpenseKey Type { get; }
        public string Details { get; set; }
        public int Amount { get; }

        public Expense(ExpenseKey type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}