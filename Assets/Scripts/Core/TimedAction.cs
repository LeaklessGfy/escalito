using System;

namespace Core
{
    public class TimedAction
    {
        private readonly float _triggerTime;
        private readonly Func<bool> _condition;
        private readonly Action _action;

        public TimedAction(float triggerTime, TimeUnit triggerUnit, Func<bool> condition, Action action)
        {
            _triggerTime = ClockController.Main.NextTime(triggerTime, triggerUnit);
            _condition = condition;
            _action = action;
            NextTime = _triggerTime;
        }

        public bool Enabled => _condition.Invoke();
        public float NextTime { get; private set; }

        public void Trigger(float currentTime)
        {
            NextTime = currentTime + _triggerTime;
            _action.Invoke();
        }
    }
}