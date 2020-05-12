using Core;

namespace Cash.CashIn
{
    public interface ITimedAction
    {
        bool Enabled { get; }
        float CurrentTime { get; set; }
        float TriggerTime { get; }
        TimeUnit Unit { get; }
        void Trigger();
    }
}