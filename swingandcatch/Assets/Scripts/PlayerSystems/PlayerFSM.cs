using System;
using TheGame.AbilitySystems;
using TheGame.AbilitySystems.Core;
using TheGame.FSM;
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
        public TransformChannelSO playerDiedChannelSO;
        public TransformChannelSO playerReachedEndChannelSO;
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
        PlayerStateFactory factory;

        public IMovementHandler movementHandler { get; private set; }
        public IRotationHandler rotationHandler { get; private set; }
        public IDamageHandler damageHandler { get; private set; }
        public IAbilityHandler abilityHandler { get; private set; }

        protected override void Awake()
        {
            movementHandler = GetComponent<IMovementHandler>();
            rotationHandler = GetComponent<IRotationHandler>();
            damageHandler = GetComponent<IDamageHandler>();
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
            ChangeState(value ? emptyState : stateBeforeEmpty);
        }

        protected override State GetInitialState()
        {
            factory = new PlayerStateFactory(this);
            return factory.GetState<PlayerGroundedState>();
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