using System;

namespace Core
{
    public class TimingAction
    {
        private float _currentTime;
        private float _triggerTime;
        private readonly Func<float> _trigger;

        public TimingAction(float triggerTime, Func<float> trigger)
        {
            _currentTime = 0;
            _triggerTime = triggerTime;
            _trigger = trigger;
        }

        public void Tick(float delta)
        {
            _currentTime += delta;
            if (_currentTime < _triggerTime)
            {
                return;
            }
            _currentTime = 0;
            _triggerTime = _trigger();
        }
    }
}