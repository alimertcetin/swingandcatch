using System;
using UnityEngine;

namespace TheGame.FSM
{
    public abstract class StateMachine : MonoBehaviour
    {
        State currentState;

        protected virtual void Awake() => InitializeStateMachine(GetInitialState());
        protected virtual void Start() => currentState.EnterState(currentState);
        protected virtual void FixedUpdate() => currentState.FixedUpdateState();
        protected virtual void Update() => currentState.UpdateState();
        protected virtual void LateUpdate() => currentState.LateUpdateState();
        protected void OnDestroy() => currentState.ExitState();

        void InitializeStateMachine(State initialState)
        {
            // TODO : Maybe we could need some initialization in here
            SetCurrentState(initialState);
        }
        
        public void SetCurrentState(State newState)
        {
            var from = currentState;
            currentState = newState;
            OnStateChanged(from);
        }

        protected virtual void OnStateChanged(State from) { }
        protected abstract State GetInitialState();

        /// <summary>
        /// Checks if current state of StateMachine same as <typeparamref name="TState"/>
        /// </summary>
        /// <typeparam name="TState">State</typeparam>
        /// <returns>True if current state is TState</returns>
        public bool IsInState<TState>() where TState : State
        {
            return currentState is TState;
        }

#if UNITY_EDITOR
        void OnGUI()
        {
            if (currentState == null) return;
            GUILayout.Label(currentState.GetType().Name, GUIStyle.none);

            foreach (State childState in currentState.childs)
            {
                GUILayout.Label(childState.GetType().Name, GUIStyle.none);
            }
        }
#endif
    }
}