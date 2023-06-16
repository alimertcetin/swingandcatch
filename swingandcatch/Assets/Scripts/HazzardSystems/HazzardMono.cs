using System;
using UnityEngine;

namespace TheGame.HazzardSystems
{
    public class HazzardMono : MonoBehaviour
    {
        public float damageAmount;
        Action<Transform> onTargetHit;

        public void RegisterHit(Action<Transform> action)
        {
            onTargetHit += action;
        }

        public void UnregisterHit(Action<Transform> action)
        {
            onTargetHit -= action;
        }

        public void RaiseEvent(Transform target)
        {
            onTargetHit?.Invoke(target);
        }
    }
}