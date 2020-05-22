using System;

namespace Core
{
    public class DelegateTimeAction : AbstractTimeAction
    {
        private readonly Func<bool> _condition;
        private readonly Action _action;

        public DelegateTimeAction(float triggerTime, TimeUnit triggerUnit, Func<bool> condition, Action action) : base(triggerTime, triggerUnit)
        {
            _condition = condition;
            _action = action;
        }

        protected override bool Condition()
        {
            return _condition.Invoke();
        }

        protected override void Action()
        {
            _action.Invoke();
        }
    }
}