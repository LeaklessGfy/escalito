using Core;

namespace Cash.Trigger
{
    public class CashTrigger
    {
        public CashTrigger(CashTriggerKey type, float triggerTime, TimeUnit triggerUnit, decimal amount, string details)
        {
            Type = type;
            TriggerTime = triggerTime;
            TriggerUnit = triggerUnit;
            Amount = amount;
            Details = details;
        }
        
        public CashTriggerKey Type { get; }
        public float TriggerTime { get; }
        public TimeUnit TriggerUnit { get; }
        public decimal Amount { get; }
        public string Details { get; }
    }
}