using System.Globalization;
using Core;

namespace Cash.Trigger
{
    public class CashTriggerAction : AbstractTimeAction
    {
        private readonly CashTrigger _cashTrigger;
        
        public CashTriggerAction(CashTrigger cashTrigger) : base(cashTrigger.TriggerTime, cashTrigger.TriggerUnit)
        {
            _cashTrigger = cashTrigger;
        }

        protected override bool Condition()
        {
            return true;
        }

        protected override void Action()
        {
            var cash = MagicBag.Bag.cash;
            cash.Cash += _cashTrigger.Amount;
            
            cash.expenseText.text = _cashTrigger.Amount.ToString(CultureInfo.InvariantCulture);
            cash.expenseText.color = PercentHelper.GetColor((cash.Cash - _cashTrigger.Amount) / cash.Cash * 100);
            
            MagicBag.Bag.audio.cash.Play();
        }
    }
}