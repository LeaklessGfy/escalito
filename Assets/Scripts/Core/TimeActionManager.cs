using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class TimeActionManager
    {
        private readonly HashSet<ITimeAction> _actions = new HashSet<ITimeAction>();

        public void Add(ITimeAction action)
        {
            _actions.Add(action);
        }

        public void Remove(ITimeAction action)
        {
            _actions.Remove(action);
        }

        public void Tick(float currentTime)
        {
            foreach (var action in _actions.Where(action => action.IsEnable(currentTime)))
            {
                action.Trigger(currentTime);
            }
        }
    }
}