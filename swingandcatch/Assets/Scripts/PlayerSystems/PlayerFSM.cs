using System;
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
        public TransformChannelSO selectableSelectChannel;
        public TransformChannelSO selectableDeselectChannel;
        public TransformChannelSO playerDiedChannelSO;
        public TransformChannelSO playerReachedEndChannelSO;
        public FloatChannelSO updatePlayerHealthChannel;
        public FloatChannelSO cameraShakeChannel;
        [SerializeField] BoolChannelSO gamePausedChannel;

        public Transform playerVisualTransform;
        [Tooltip("Left to Right order")]
        public Transform[] playerFeet;
        public Transform playerSword;

        public Vector3 bottomColliderPosLocal = Vector3.zero;
        public Vector3 bottomColliderSize = Vector3.one;

        public Vector3 velocity { get; private set; }
        public Vector3 previousPosition { get; private set; }
        public CircleCollider2D circleCollider2D { get; private set; }

        [HideInInspector] public bool damageImmune;
        public Health health;
        
        // TODO : Remove this states from here
        State stateBeforeEmpty;
        EmptyState emptyState;

        protected override void Awake()
        {
            previousPosition = transform.position;
            circleCollider2D = GetComponent<CircleCollider2D>();
            health = healthSO.GetHealth();
            emptyState = new EmptyState(this);
            base.Awake();
        }

        protected override void Update()
        {
            var position = transform.position;
            velocity = position - previousPosition;
            previousPosition = position;
            base.Update();
        }

        void OnEnable()
        {
            gamePausedChannel.Register(OnGamePaused);
        }

        void OnDisable()
        {
            gamePausedChannel.Unregister(OnGamePaused);
        }

        void OnGamePaused(bool value)
        {
            if (value)
            {
                stateBeforeEmpty = currentState;
                stateBeforeEmpty.ExitState();
                SetCurrentState(emptyState);
            }
            else
            {
                SetCurrentState(stateBeforeEmpty);
                stateBeforeEmpty.EnterState(emptyState);
            }
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
            cameraShakeChannel.RaiseEvent(10f);
            updatePlayerHealthChannel.RaiseEvent(health.normalized);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (stateDatas.attackStateDataSO)
            {
                XIV.Core.XIVDebug.DrawCircle(transform.position, stateDatas.attackStateDataSO.attackRadius, Color.red);
            }
            
            if (Application.isPlaying) return;
            var pos = transform.position;
            var bottomColliderPos = pos + bottomColliderPosLocal;
            var sizeHalf = bottomColliderSize * 0.5f;
            var radius = sizeHalf.y;
            XIV.Core.XIVDebug.DrawCircle(bottomColliderPos + Vector3.left * (sizeHalf.x - radius * 0.5f), radius);
            XIV.Core.XIVDebug.DrawCircle(bottomColliderPos + Vector3.right * (sizeHalf.x - radius * 0.5f), radius);
            XIV.Core.XIVDebug.DrawRectangle(bottomColliderPos, sizeHalf, Quaternion.LookRotation(Vector3.forward));
            XIV.Core.XIVDebug.DrawCircle(bottomColliderPos + Vector3.down * sizeHalf.y, radius);
        }
#endif
    }
}