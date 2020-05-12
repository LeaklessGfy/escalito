namespace Cash.CashIn
{
    public class CashIn : ITimedAction
    {
        public CashIn(float triggerTime, decimal amount)
        {
            CurrentTime = 0;
            TriggerTime = triggerTime;
            Amount = amount;
        }

        public bool Enabled { get; } = true;
        public float CurrentTime { get; set; }
        public float TriggerTime { get; }
        public decimal Amount { get; }

        public void Trigger()
        {
            CashController.Main.Cash += Amount;
        }
    }
}