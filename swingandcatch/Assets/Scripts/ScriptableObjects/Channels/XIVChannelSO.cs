using System;
using UnityEngine;

namespace TheGame.ScriptableObjects.Channels
{
    public abstract class XIVChannelSO : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField, TextArea] string description;
#endif
        Action action;

        public void Register(Action action) => this.action += action;
        public void Unregister(Action action) => this.action -= action;
        public void RaiseEvent() => action?.Invoke();
    }
    
    public abstract class XIVChannelSO<T> : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField, TextArea] string description;
#endif
        Action<T> action;

        public void Register(Action<T> action) => this.action += action;
        public void Unregister(Action<T> action) => this.action -= action;
        public void RaiseEvent(T value) => action?.Invoke(value);
    }
}