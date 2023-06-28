using System;
using UnityEngine;

namespace TheGame.HazzardSystems
{
    public class HazzardMono : MonoBehaviour
    {
        public float damageAmount;
        Action<HazzardMono, Transform> onTargetHit;

        public void RegisterHit(Action<HazzardMono, Transform> action)
        {
            onTargetHit += action;
        }

        public void UnregisterHit(Action<HazzardMono, Transform> action)
        {
            onTargetHit -= action;
        }

        public void RaiseEvent(Transform target)
        {
            onTargetHit?.Invoke(this, target);
        }
    }
}