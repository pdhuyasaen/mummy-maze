using System.Linq;
using Dacodelaac.DebugUtils;

namespace Dacodelaac.FiniteStateMachine
{
    public class StateMachine<T> where T : State
    {
        public T CurrentState { get; private set; }

        public T[] States { get; private set; }

        public void InitStates(params T[] states)
        {
            States = states;
        }

        public void ChangeState<TK>(object data = null) where TK : T
        {
            var state = States.FirstOrDefault(s => s is TK);
            ChangeState(state, data);
        }

        public void ChangeState(T state, object data = null)
        {
            if (state == CurrentState) return;
            
            Dacoder.Log($"State: {state.GetType()}");
            
            var oldState = CurrentState;
            CurrentState = state;
            oldState?.StateExit(state);
            state?.StateEnter(oldState, data);
        }

        public void Update()
        {
            CurrentState?.StateUpdate();
        }

        public void FixedUpdate()
        {
            CurrentState?.StateFixedUpdate();
        }
    }
}