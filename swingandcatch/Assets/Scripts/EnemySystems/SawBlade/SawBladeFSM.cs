using System;
using TheGame.EnemySystems.SawBlade.States;
using TheGame.FSM;
using TheGame.HealthSystems;
using TheGame.ScriptableObjects.HealthSystems;
using TheGame.ScriptableObjects.StateDatas.SawBladeStateDatas;
using UnityEngine;
using XIV.Core;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;

namespace TheGame.EnemySystems.SawBlade
{
    public class SawBladeFSM : StateMachine, IDamageable
    {
        [SerializeField] HealthSO healthSO;
        [SerializeField] MeshRenderer heathbar;
        public SawBladeIdleStateDataSO idleStateDataSO;
        public SawBladeTransitionToIdleStateDataSO transitionToIdleStateDataSO;
        public SawBladeAttackStateDataSO attackStateDataSO;
        
        [NonSerialized] public Vector3 idleStartPosition;
        public Vector3 idleEndPosition => idleStartPosition - idleStateDataSO.idleMovementAxis * idleStateDataSO.idleMovementDistance;
        public Collider2D collider2D { get; private set; }
        public Health health;

        protected override void Awake()
        {
            idleStartPosition = transform.position + idleStateDataSO.idleMovementAxis.normalized * (idleStateDataSO.idleMovementDistance * 0.5f);
            collider2D = GetComponent<Collider2D>();
            health = healthSO.GetHealth();
            heathbar.material.SetFloat(ShaderConstants.Unlit_HealthbarShader.Health_RangeID, health.normalized);
            base.Awake();
        }

        protected override State GetInitialState()
        {
            return new SawBladeStateFactory(this).GetState<SawBladeIdleTransitionState>();
        }

        bool IDamageable.CanReceiveDamage() => health.isDepleted == false;

        void IDamageable.ReceiveDamage(float amount)
        {
            health.DecreaseCurrentHealth(amount);
            heathbar.material.SetFloat(ShaderConstants.Unlit_HealthbarShader.Health_RangeID, health.normalized);
            if (health.isDepleted)
            {
                base.SetCurrentState(new EmptyState(this));
                transform.XIVTween()
                    .Scale(transform.localScale, Vector3.one * 0.5f, 0.5f, EasingFunction.EaseInOutBounce, true, 1)
                    .OnComplete(() => Destroy(this.gameObject))
                    .Start();
            }
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