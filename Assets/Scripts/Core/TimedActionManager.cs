using System.Collections.Generic;

namespace Core
{
    public class TimedActionManager
    {
        private readonly List<TimedAction> _actions = new List<TimedAction>();

        public void Add(TimedAction action)
        {
            _actions.Add(action);
        }

        public void Tick(float currentTime)
        {
            foreach (var action in _actions)
            {
                if (!action.Enabled || currentTime < action.NextTime)
                {
                    return;
                }
                action.Trigger(currentTime);
            }
        }
    }
}