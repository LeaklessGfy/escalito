namespace Core
{
    public class Expense
    {
        public Expense(ExpenseKey type, int amount)
        {
            Type = type;
            Amount = amount;
        }

        public ExpenseKey Type { get; }
        public string Details { get; set; }
        public int Amount { get; }
    }
}