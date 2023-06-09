using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using XIV.Core.Extensions;

namespace TheGame.FSM
{
    public abstract class State
    {
        protected State previousState;
        protected State parentState;
        List<State> childStates;
        StateMachine stateMachine;
#if UNITY_EDITOR
        public IReadOnlyCollection<State> childs => childStates.AsReadOnly();
#endif

        public State(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void EnterState(State comingFrom)
        {
#if UNITY_EDITOR
            if (FSMDebugSettings.IsStateChangeLogsEnabled)
            {
                Debug.LogWarning(nameof(OnStateEnter).ToColorGreen() + " = " + this.GetType().Name);
            }
#endif
            previousState = comingFrom;
            childStates = ListPool<State>.Get();
            InitializeChildStates();
            OnStateEnter(comingFrom);
            
            int childStateCount = childStates.Count;
            for (int i = 0; i < childStateCount; i++)
            {
                childStates[i].EnterState(this);
            }
        }
        
        public void FixedUpdateState()
        {
            int childStateCount = childStates.Count;
            for (int i = 0; i < childStateCount; i++)
            {
                childStates[i].FixedUpdateState();
            }
            OnStateFixedUpdate();
        }

        /// <summary>
        /// Update is called once per evey frame
        /// </summary>
        public void UpdateState()
        {
            int childStateCount = childStates.Count;
            for (int i = 0; i < childStateCount; i++)
            {
                childStates[i].UpdateState();
            }
            OnStateUpdate();
            CheckTransitions();
        }
        
        public void LateUpdateState()
        {
            int childStateCount = childStates.Count;
            for (int i = 0; i < childStateCount; i++)
            {
                childStates[i].LateUpdateState();
            }
            OnStateLateUpdate();
        }

        public void ExitState()
        {
#if UNITY_EDITOR
            if (FSMDebugSettings.IsStateChangeLogsEnabled)
            {
                Debug.LogWarning(nameof(OnStateExit).ToColorRed() + " = " + this.GetType().Name);
            }
#endif
            int childStateCount = childStates.Count;
            for (int i = 0; i < childStateCount; i++)
            {
                childStates[i].ExitState();
            }
            ListPool<State>.Release(childStates);
            OnStateExit();
        }

        // --- Life Cycle Callbacks
        protected virtual void OnStateEnter(State comingFrom) { }
        protected virtual void OnStateFixedUpdate() { }
        protected virtual void OnStateUpdate() { }
        protected virtual void OnStateLateUpdate() { }
        protected virtual void OnStateExit() { }
        protected virtual void InitializeChildStates() { }

        /// <summary>
        /// CheckTransitions is called in every <see cref="UpdateState"/> loop
        /// </summary>
        protected virtual void CheckTransitions() { }

        protected void AddChildState(State childState)
        {
            childState.SetParentState(this);
            childStates.Add(childState);
        }

        protected void RemoveChildState(State subState) => childStates.Remove(subState);

        protected void SetParentState(State parentState)
        {
            this.parentState?.RemoveChildState(this);
            this.parentState = parentState;
        }

        /// <summary>
        /// Call this if State is a parent state
        /// </summary>
        /// <param name="newState"></param>
        protected void ChangeRootState(State newState)
        {
            if (parentState != null) parentState.ExitState();
            else ExitState();

            stateMachine.SetCurrentState(newState);
            newState?.EnterState(this);
        }

        /// <summary>
        /// Use this if state is a child state
        /// </summary>
        /// <param name="newState"></param>
        protected void ChangeChildState(State newState)
        {
            ExitState();
            parentState.RemoveChildState(this);
            parentState.AddChildState(newState);
            newState.EnterState(this);
        }
    }

    public class State<TStateMachine, TStateFactory> : State 
        where TStateMachine : StateMachine 
        where TStateFactory : StateFactory<TStateMachine>
    {
        protected TStateMachine stateMachine;
        protected TStateFactory factory;
        
        public State(TStateMachine stateMachine, TStateFactory stateFactory) : base(stateMachine)
        {
            this.stateMachine = stateMachine;
            this.factory = stateFactory;
        }
    }
}