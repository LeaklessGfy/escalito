using Cash.Effect;
using Cash.Expense;

namespace Cash
{
    public class Contract
    {
        private Contract(decimal price, IExpense expense, IEffect bonus, IEffect penalty)
        {
            Price = price;
            Expense = expense;
            Bonus = bonus;
            Penalty = penalty;
        }

        public decimal Price { get; }
        public IExpense Expense { get; }
        public IEffect Bonus { get; }
        public IEffect Penalty { get; }

        public static Contract Build(MainController mainController)
        {
            var price = 100;
            var expense = new ContractExpense(price);
            var bonus = new ComboBonus(mainController, 1.5m);
            var penalty = new SimpleEffect(price, (customer, amount) => amount + price);

            return new Contract(price, expense, bonus, penalty);
        }
    }
}