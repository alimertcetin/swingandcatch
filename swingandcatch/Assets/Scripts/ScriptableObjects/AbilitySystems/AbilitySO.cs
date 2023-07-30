using TheGame.AbilitySystems.Core;
using UnityEngine;

namespace TheGame.ScriptableObjects.AbilitySystems
{
    public abstract class AbilitySO : ScriptableObject
    {
        public abstract IAbility GetAbility();
    }

    public abstract class AbilitySO<T> : AbilitySO where T : IAbility
    {
        [SerializeField] protected T abiility;
    }
}