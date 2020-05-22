using Cash.Effect;
using Cash.Trigger;
using Core;

namespace Cash.Contract
{
    public static class ContractBuilder
    {
        public static Contract Build(MainController mainController)
        {
            var price = 100 * mainController.Difficulty;
            var triggerTime = 5 * mainController.Difficulty;
            var triggerAmount = 1000 * mainController.Difficulty;
            var comboAmount = 1.5m * mainController.Difficulty;
            var penaltyAmount = triggerAmount * 10 * mainController.Difficulty;
            
            var cashTrigger = new CashTrigger(CashTriggerKey.Contracts, triggerTime, TimeUnit.Minute, triggerAmount, "");
            var bonus = new ComboBonus(mainController, comboAmount);
            var penalty = new SimpleEffect(penaltyAmount, (customer, amount) => amount + penaltyAmount);

            return new Contract(price, cashTrigger, bonus, penalty);
        }
    }
}