using System.Collections.Generic;
using Cash.CashIn;

namespace Core
{
    public class TimedActionManager
    {
        private readonly List<ITimedAction> _actions = new List<ITimedAction>();

        public void Add(ITimedAction action)
        {
            _actions.Add(action);
        }

        public void Tick(float delta)
        {
            foreach (var action in _actions)
            {
                if (!action.Enabled)
                {
                    return;
                }
                
                action.CurrentTime += delta;

                if (action.CurrentTime < action.TriggerTime)
                {
                    return;
                }

                action.Trigger();
            }
        }
    }
}