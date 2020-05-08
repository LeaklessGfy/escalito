using System;

namespace Core
{
    public class TimingAction
    {
        private readonly Func<bool> _condition;
        private readonly Action<float, float> _tick;
        private readonly Func<float> _trigger;
        private float _currentTime;
        private float _triggerTime;

        public TimingAction(float currentTime, float triggerTime, Func<bool> condition, Action<float, float> tick, Func<float> trigger)
        {
            _currentTime = currentTime;
            _triggerTime = triggerTime;
            _condition = condition;
            _tick = tick;
            _trigger = trigger;
        }

        public TimingAction(float triggerTime, Func<bool> condition, Action<float, float> tick, Func<float> trigger) : this(0, triggerTime, condition, null, trigger)
        {
        }

        public TimingAction(float triggerTime, Func<bool> condition, Func<float> trigger) : this(0, triggerTime, condition, null, trigger)
        {
        }

        public void Tick(float delta)
        {
            if (!_condition.Invoke())
            {
                return;
            }

            _currentTime += delta;
            _tick?.Invoke(_currentTime, _triggerTime);

            if (_currentTime < _triggerTime)
            {
                return;
            }

            _currentTime = 0;
            _triggerTime = _trigger();
        }
    }
}