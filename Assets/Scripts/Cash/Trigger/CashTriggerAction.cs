using System.Globalization;
using Core;

namespace Cash.Trigger
{
    public class CashTriggerAction : AbstractTimeAction
    {
        private readonly CashTrigger _cashTrigger;
        private readonly CashController _cashController;
        
        public CashTriggerAction(CashTrigger cashTrigger, CashController cashController) : base(cashTrigger.TriggerTime, cashTrigger.TriggerUnit)
        {
            _cashTrigger = cashTrigger;
            _cashController = cashController;
        }

        protected override bool Condition()
        {
            return true;
        }

        protected override void Action()
        {
            _cashController.Cash += _cashTrigger.Amount;
            
            _cashController.expenseText.text = _cashTrigger.Amount.ToString(CultureInfo.InvariantCulture);
            _cashController.expenseText.color = PercentHelper.GetColor((_cashController.Cash - _cashTrigger.Amount) / _cashController.Cash * 100);
            
            AudioController.Main.cash.Play();
        }
    }
}