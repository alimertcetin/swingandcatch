using System;
using TheGame.CoinSystems;
using TheGame.EnemySystems.SawBlade.States;
using TheGame.FSM;
using TheGame.HealthSystems;
using TheGame.ScriptableObjects.HealthSystems;
using TheGame.ScriptableObjects.StateDatas.SawBladeStateDatas;
using TheGame.SelectionSystems;
using UnityEngine;
using XIV.Core;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using Random = UnityEngine.Random;

namespace TheGame.EnemySystems.SawBlade
{
    public class SawBladeFSM : StateMachine, IDamageable, ISelectable
    {
        [SerializeField] HealthSO healthSO;
        [SerializeField] MeshRenderer heathbar;
        [SerializeField] GameObject selectionIndicator;
        
        public SawBladeIdleStateDataSO idleStateDataSO;
        public SawBladeTransitionToIdleStateDataSO transitionToIdleStateDataSO;
        public SawBladeAttackStateDataSO attackStateDataSO;
        
        [SerializeField] bool enableDrops = true;
        [Header("Drops")]
        [SerializeField] Coin coinWithRigidbodyPrefab;
        
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
            heathbar.gameObject.SetActive(false);
            base.Awake();
        }

        protected override State GetInitialState()
        {
            return new SawBladeStateFactory(this).GetState<SawBladeIdleTransitionState>();
        }

        bool IDamageable.CanReceiveDamage() => health.isDepleted == false;

        void IDamageable.ReceiveDamage(float amount)
        {
            // Otherwise we can call ReceiveDamage multiple times and that can cause Destroy call multiple times and throws exception
            if (health.isDepleted) return;
            
            transform.CancelTween();
            transform.XIVTween()
                .ScaleX(1f, 0.65f, 0.25f, EasingFunction.EaseOutBounce, true)
                .Start();
            
            health.DecreaseCurrentHealth(amount);
            heathbar.material.SetFloat(ShaderConstants.Unlit_HealthbarShader.Health_RangeID, health.normalized);
            if (health.isDepleted)
            {
                base.SetCurrentState(new EmptyState(this));
                transform.XIVTween()
                    .Scale(transform.localScale, Vector3.one * 0.5f, 0.5f, EasingFunction.EaseInOutBounce, true, 1)
                    .OnComplete(() => Destroy(this.gameObject))
                    .Start();
                
                if (enableDrops)
                {
                    SpawnDrops();
                }
            }

        }

        void SpawnDrops()
        {
            var coinAmount = Mathf.RoundToInt(Random.value * 5f);
            var pos = transform.position;
            pos.z = 0f;
            for (int i = 0; i < coinAmount; i++)
            {
                var coin = Instantiate(coinWithRigidbodyPrefab, pos, Quaternion.identity);
                var force = Random.insideUnitCircle * (Random.value * 10f);
                coin.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
            }
        }

        SelectionSettings ISelectable.GetSelectionSettings()
        {
            return new SelectionSettings
            {
                duration = 1.5f,
            };
        }

        void ISelectable.OnSelect()
        {
            selectionIndicator.transform.CancelTween();
            selectionIndicator.SetActive(true);
            heathbar.gameObject.SetActive(true);
            selectionIndicator.transform.XIVTween()
                .Scale(Vector3.one, Vector3.one * 1.5f, 0.5f, EasingFunction.SmoothStop3, true, 2)
                .Start();
        }

        void ISelectable.OnDeselect()
        {
            selectionIndicator.transform.CancelTween();
            selectionIndicator.transform.XIVTween()
                .Scale(Vector3.one, Vector3.zero, 0.5f, EasingFunction.EaseOutExpo)
                .OnComplete(() =>
                {
                    heathbar.gameObject.SetActive(false);
                    selectionIndicator.SetActive(false);
                })
                .Start();
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