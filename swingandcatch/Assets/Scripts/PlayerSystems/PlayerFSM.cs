using TheGame.AbilitySystems.Core;
using TheGame.FSM;
using TheGame.HealthSystems;
using TheGame.Interfaces;
using TheGame.PlayerSystems.States;
using TheGame.ScriptableObjects.Channels;
using TheGame.ScriptableObjects.StateDatas.PlayerStateDatas;
using UnityEngine;

namespace TheGame.PlayerSystems
{
    public class PlayerFSM : StateMachine, IAbilityUser
    {
        public PlayerStateDataCollectionSO stateDatas;
        
        [Header("Channels")]
        public TransformChannelSO selectableSelectChannel;
        public TransformChannelSO selectableDeselectChannel;
        [SerializeField] BoolChannelSO gamePausedChannel;

        public Transform playerVisualTransform;
        [Tooltip("Left to Right order")]
        public Transform[] playerFeet;
        public Transform playerSword;

        public Vector3 bottomColliderPosLocal = Vector3.zero;
        public Vector3 bottomColliderSize = Vector3.one;
        
        // TODO : Remove this states from here
        State stateBeforeEmpty;
        EmptyState emptyState;

        [HideInInspector] public bool isReachedEndGate;
        public IMovementHandler movementHandler { get; private set; }
        public IRotationHandler rotationHandler { get; private set; }
        public IDamageable damageable { get; private set; }
        public IAbilityHandler abilityHandler { get; private set; }

        protected override void Awake()
        {
            movementHandler = GetComponent<IMovementHandler>();
            rotationHandler = GetComponent<IRotationHandler>();
            damageable = GetComponent<IDamageable>();
            abilityHandler = GetComponent<IAbilityHandler>();
            
            emptyState = new EmptyState(this);
            base.Awake();
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
                ChangeState(emptyState);
            }
            else
            {
                ChangeState(stateBeforeEmpty);
            }
        }

        protected override State GetInitialState()
        {
            return new PlayerStateFactory(this).GetState<PlayerGroundedState>();
        }

        void IAbilityUser.BeginUse(IAbility ability)
        {
        }

        void IAbilityUser.EndUse(IAbility ability)
        {
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