namespace Core
{
    public abstract class AbstractTimeAction : ITimeAction
    {
        private readonly float _triggerTime;
        private float _nextTime;

        protected AbstractTimeAction(float triggerTime, TimeUnit triggerUnit)
        {
            _triggerTime = ClockController.Main.NextTime(triggerTime, triggerUnit);
            _nextTime = _triggerTime;
        }

        public bool IsEnable(float currentTime)
        {
            return Condition() && currentTime < _nextTime;
        }

        public void Trigger(float currentTime)
        {
            _nextTime = currentTime + _triggerTime;
            Action();
        }

        protected abstract bool Condition();
        protected abstract void Action();
    }
}
