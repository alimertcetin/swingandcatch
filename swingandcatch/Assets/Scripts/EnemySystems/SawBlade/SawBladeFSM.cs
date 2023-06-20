using System;
using TheGame.EnemySystems.SawBlade.States;
using TheGame.FSM;
using TheGame.ScriptableObjects.StateDatas.SawBladeStateDatas;
using UnityEngine;
using XIV.Core;
using XIV.Core.Utils;

namespace TheGame.EnemySystems.SawBlade
{
    public class SawBladeFSM : StateMachine
    {
        public SawBladeIdleStateDataSO idleStateDataSO;
        public SawBladeTransitionToIdleStateDataSO transitionToIdleStateDataSO;
        public SawBladeAttackStateDataSO attackStateDataSO;
        
        [NonSerialized] public Vector3 idleStartPosition;
        public Vector3 idleEndPosition => idleStartPosition - idleStateDataSO.idleMovementAxis * idleStateDataSO.idleMovementDistance;

        protected override void Awake()
        {
            idleStartPosition = transform.position + idleStateDataSO.idleMovementAxis.normalized * (idleStateDataSO.idleMovementDistance * 0.5f);
            base.Awake();
        }

        protected override State GetInitialState()
        {
            return new SawBladeStateFactory(this).GetState<SawBladeIdleTransitionState>();
        }

#if UNITY_EDITOR
        bool isCached;
        Vector3 cachedWorldPos;

        void OnEnable()
        {
            isCached = true;
            cachedWorldPos = transform.position;
        }
        
        void OnDisable() => isCached = false;
        
        void OnDrawGizmosSelected()
        {
            Vector3 position = isCached ? cachedWorldPos : transform.position;
            if (idleStateDataSO)
            {
                var moveDistanceHalf = idleStateDataSO.idleMovementDistance * 0.5f;
                var axisNormalized = idleStateDataSO.idleMovementAxis.normalized;
                var movementStart = position + axisNormalized * moveDistanceHalf;
                var movementEnd = position - axisNormalized * moveDistanceHalf;

                XIVDebug.DrawLine(movementStart, movementEnd);
                XIVDebug.DrawCircle(movementStart, 0.25f, Vector3.forward, Color.green, 5);
                XIVDebug.DrawCircle(movementEnd, 0.25f, Vector3.forward, Color.red, 5);
            }

            if (attackStateDataSO) XIVDebug.DrawCircle(position, attackStateDataSO.attackFieldRadius * 2f);
        }
#endif
    }
}