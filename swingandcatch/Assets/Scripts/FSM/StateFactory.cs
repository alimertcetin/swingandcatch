using System;
using System.Collections.Generic;

namespace TheGame.FSM
{
    public abstract class StateFactory
    {
        protected readonly StateMachine stateMachine;
        protected readonly Dictionary<Type, State> dic = new();

        public StateFactory(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public T GetState<T>() where T : State
        {
            return (T)dic[typeof(T)];
        }

        protected void AddState<T>(T state) where T : State
        {
            dic.Add(typeof(T), state);
        }
    }

    public abstract class StateFactory<T> : StateFactory where T : StateMachine
    {
        public new readonly T stateMachine;
        
        public StateFactory(T stateMachine) : base(stateMachine)
        {
            this.stateMachine = stateMachine;
        }
    }
}