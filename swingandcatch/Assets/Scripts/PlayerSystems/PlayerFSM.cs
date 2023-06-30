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

        // TODO : Move input to somewhere else, consider implementing InputSystem instead of using legacy
        public bool hasHorizontalMovementInput => Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0f;
        public Vector3 horizontalMovementInput => new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
        public bool isRunPressed => Input.GetKey(KeyCode.LeftShift);
        public bool isJumpPressed => Input.GetKey(KeyCode.Space);

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
    }
}