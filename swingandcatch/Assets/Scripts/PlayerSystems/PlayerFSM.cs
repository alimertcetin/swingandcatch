using TheGame.FSM;
using TheGame.HealthSystems;
using TheGame.PlayerSystems.States;
using TheGame.ScriptableObjects.Channels;
using TheGame.ScriptableObjects.HealthSystems;
using TheGame.ScriptableObjects.StateDatas.PlayerStateDatas;
using UnityEngine;

namespace TheGame.PlayerSystems
{
    public class PlayerFSM : StateMachine, IDamageable
    {
        public PlayerStateDataCollectionSO stateDatas;

        [Header("Health and Damage Settings")]
        [SerializeField] HealthSO healthSO;
        public float damageImmuneDuration = 5f;
        
        [Header("Channels")]
        public TransformChannelSO playerDiedChannelSO;
        public TransformChannelSO playerReachedEndChannelSO;
        public FloatChannelSO updatePlayerHealthChannel;

        public Transform playerVisualTransform;
        [Tooltip("Left to Right order")]
        public Transform[] playerFeet;

        public Vector3 bottomColliderPosLocal = Vector3.zero;
        public Vector3 bottomColliderSize = Vector3.one;

        public Vector3 velocity { get; private set; }
        public Vector3 previousPosition { get; private set; }
        public CircleCollider2D circleCollider2D { get; private set; }

        [HideInInspector] public bool damageImmune;
        public Health health;

        protected override void Awake()
        {
            previousPosition = transform.position;
            circleCollider2D = GetComponent<CircleCollider2D>();
            health = healthSO.GetHealth();
            base.Awake();
        }

        protected override void Update()
        {
            var position = transform.position;
            velocity = position - previousPosition;
            previousPosition = position;
            base.Update();
        }

        protected override State GetInitialState()
        {
            return new PlayerStateFactory(this).GetState<PlayerGroundedState>();
        }

        public void SyncPosition()
        {
            previousPosition = transform.position;
        }

        public bool Move(Vector3 targetPosition)
        {
            var collisionMask = 1 << PhysicsConstants.GroundLayer;
            var distance = stateDatas.groundedStateDataSO.groundCheckDistance;
            
            var canMove = this.CanMove(targetPosition, circleCollider2D.bounds, distance, collisionMask, out var positionBeforeCollision);
            var nextPos = canMove ? targetPosition : positionBeforeCollision;
            transform.position = nextPos;
            
            return canMove;
        }

        bool IDamageable.CanReceiveDamage()
        {
            return damageImmune == false;
        }

        void IDamageable.ReceiveDamage(float amount)
        {
            if (damageImmune) return;
            health.DecreaseCurrentHealth(amount);
            updatePlayerHealthChannel.RaiseEvent(health.normalized);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) return;
            var pos = transform.position;
            var bottomColliderPos = pos + bottomColliderPosLocal;
            var sizeHalf = bottomColliderSize * 0.5f;
            var radius = sizeHalf.y * 2f;
            XIV.Core.XIVDebug.DrawCircle(bottomColliderPos + Vector3.left * (sizeHalf.x - radius * 0.5f), radius);
            XIV.Core.XIVDebug.DrawCircle(bottomColliderPos + Vector3.right * (sizeHalf.x - radius * 0.5f), radius);
            XIV.Core.XIVDebug.DrawRectangle(bottomColliderPos, sizeHalf, Quaternion.LookRotation(Vector3.forward));
            XIV.Core.XIVDebug.DrawCircle(bottomColliderPos + Vector3.down * sizeHalf.y, radius);
        }
#endif
    }
}