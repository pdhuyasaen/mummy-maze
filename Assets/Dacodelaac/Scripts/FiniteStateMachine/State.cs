using System;
using Dacodelaac.DebugUtils;

namespace Dacodelaac.FiniteStateMachine
{
    public class State
    {
        public void StateEnter(State from, object data)
        {
            OnStateEnter(from, data);
        }

        public void StateUpdate()
        {
            OnStateUpdate();
        }

        public void StateFixedUpdate()
        {
            OnStateFixedUpdate();
        }

        public void StateExit(State to)
        {
            OnStateExit(to);
        }

        protected virtual void OnStateEnter(State from, object data)
        {
        }

        protected virtual void OnStateUpdate()
        {
        }

        protected virtual void OnStateFixedUpdate()
        {
        }

        protected virtual void OnStateExit(State to)
        {
        }
    }
}