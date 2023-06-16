using TheGame.EnemySystems.SawBlade.States;
using TheGame.FSM;
using UnityEngine;
using XIV.Core;
using XIV.Core.Utils;

namespace TheGame.EnemySystems.SawBlade
{
    public class SawBladeFSM : StateMachine
    {
        [Header("Idle")]
        public Vector3 idleMovementAxis = Vector3.right;
        public float idleMovementDistance = 2f;
        public float idleMovementSpeed = 2f;
        public EasingFunction.Ease ease;
        public Vector3 idleStartPosition { get; private set; }
        public Vector3 idleEndPosition => idleStartPosition - idleMovementAxis * idleMovementDistance;
        
        [Header("Transition To Idle")]
        public float goToStartPositionSpeed = 5f;

        [Header("Attack")]
        public float attackDuration = 1f;
        public float followDistance = 1.5f;
        public float followSpeed = 2;
        public float attackFieldRadius = 2f;
        public GameObject projectilePrefab;

        protected override void Awake()
        {
            idleStartPosition = transform.position + idleMovementAxis.normalized * (idleMovementDistance * 0.5f);
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
            var moveDistanceHalf = idleMovementDistance * 0.5f;
            var axisNormalized = idleMovementAxis.normalized;
            var movementStart = position + axisNormalized * moveDistanceHalf;
            var movementEnd = position - axisNormalized * moveDistanceHalf;
            
            XIVDebug.DrawLine(movementStart, movementEnd);
            XIVDebug.DrawCircle(movementStart, 0.25f, Vector3.forward, Color.green, 5);
            XIVDebug.DrawCircle(movementEnd, 0.25f, Vector3.forward, Color.red, 5);
            
            XIVDebug.DrawCircle(position, attackFieldRadius * 2f);
        }
#endif
    }
}