using System.Collections.Generic;

namespace Characters
{
    public enum StateKey
    {
        Idle,
        Move,
        Wait,
        Leave,
        Exhaust
    }

    public class State
    {
        private readonly HashSet<StateKey> _states = new HashSet<StateKey> {StateKey.Idle};

        public bool Idling => _states.Contains(StateKey.Idle);
        public bool Moving => _states.Contains(StateKey.Move);
        public bool Waiting => _states.Contains(StateKey.Wait);
        public bool Leaving => _states.Contains(StateKey.Leave);
        public bool Exhausted => _states.Contains(StateKey.Exhaust);

        public void Idle()
        {
            _states.Remove(StateKey.Move);
            _states.Add(StateKey.Idle);
        }

        public void Move()
        {
            _states.Remove(StateKey.Idle);
            _states.Add(StateKey.Move);
        }

        public void Wait()
        {
            _states.Remove(StateKey.Exhaust);
            _states.Add(StateKey.Wait);
        }

        public void UnWait()
        {
            _states.Remove(StateKey.Wait);
        }

        public void Leave()
        {
            _states.Remove(StateKey.Wait);
            _states.Add(StateKey.Leave);
        }

        public void Exhaust()
        {
            _states.Remove(StateKey.Wait);
            _states.Add(StateKey.Exhaust);
        }
    }
}