using Cash.Effect;
using Cash.Trigger;
using Core;

namespace Cash.Contract
{
    public static class ContractBuilder
    {
        public static Contract Build()
        {
            var main = MagicBag.Bag.main;
            
            var price = 100 * main.Difficulty;
            var triggerTime = 5 * main.Difficulty;
            var triggerAmount = 1000 * main.Difficulty;
            var comboAmount = 1.5m * main.Difficulty;
            var penaltyAmount = triggerAmount * 10 * main.Difficulty;
            
            var cashTrigger = new CashTrigger(CashTriggerKey.Contracts, triggerTime, TimeUnit.Minute, triggerAmount, "");
            var bonus = new SimpleEffect(comboAmount,  (customer, amount) => amount + (main.PositiveCombo * comboAmount));
            var penalty = new SimpleEffect(penaltyAmount, (customer, amount) => amount + penaltyAmount);

            return new Contract(price, cashTrigger, bonus, penalty);
        }
    }
}