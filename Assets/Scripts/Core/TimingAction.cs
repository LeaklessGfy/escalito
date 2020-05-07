using System;

namespace Core
{
    public class TimingAction
    {
        private readonly Action<float, float> _tick;
        private readonly Func<float> _trigger;
        private float _currentTime;
        private float _triggerTime;

        public TimingAction(float currentTime, float triggerTime, Action<float, float> tick, Func<float> trigger)
        {
            _currentTime = currentTime;
            _triggerTime = triggerTime;
            _tick = tick;
            _trigger = trigger;
        }

        public TimingAction(float triggerTime, Func<float> trigger) : this(0, triggerTime, null, trigger)
        {
        }

        public TimingAction(float triggerTime, Action<float, float> tick, Func<float> trigger) : this(0, triggerTime,
            tick, trigger)
        {
        }

        public void Tick(float delta)
        {
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