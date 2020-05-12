using System;
using Cash.CashIn;

namespace Core
{
    public class TimedAction : ITimedAction
    {
        private Func<bool> _condition;
        private Action _trigger;

        public TimedAction(float triggerTime, Func<bool> condition, Action trigger)
        {
            CurrentTime = 0;
            TriggerTime = triggerTime;
            _condition = condition;
            _trigger = trigger;
        }

        public bool Enabled => _condition.Invoke();
        public float CurrentTime { get; set; }
        public float TriggerTime { get; }

        public void Trigger()
        {
            _trigger.Invoke();
        }
    }
}