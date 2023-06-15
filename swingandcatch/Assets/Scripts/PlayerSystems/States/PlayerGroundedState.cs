using System.Buffers;
using TheGame.FSM;
using TheGame.PlayerSystems.States.DamageStates;
using UnityEngine;

namespace TheGame.PlayerSystems.States
{
    public class PlayerGroundedState : State<PlayerFSM, PlayerStateFactory>
    {
        public PlayerGroundedState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            if (SetGroundedPosition()) return;
#if UNITY_EDITOR
            if (FSMDebugSettings.IsStateChangeLogsEnabled) Debug.LogError("Player is not grounded but still made a transition to the GroundedState from " + comingFrom.GetType().Name);
#endif
        }

        protected override void InitializeChildStates()
        {
            AddChildState(factory.GetState<PlayerIdleState>());
            AddChildState(factory.GetState<CheckDamageState>());
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.isJumpPressed)
            {
                ChangeRootState(factory.GetState<PlayerJumpState>());
                return;
            }
            
            if (stateMachine.IsGrounded() == false)
            {
                ChangeRootState(factory.GetState<PlayerFallingState>());
                return;
            }
        }

        bool SetGroundedPosition()
        {
            const int DETAIL = 10;
            const float ERROR = 0.1f;
            var transform = this.stateMachine.transform;
            var down = -transform.up;
            var currentPosition = transform.position;
            var localScaleYHalf = transform.localScale.y * 0.5f - ERROR;
            var previousPosition = stateMachine.previousPosition;
            var castStartPosition = previousPosition + down * localScaleYHalf;
            var groundCheckDistance = stateMachine.groundCheckDistance;

            var raycastHitBuffer = ArrayPool<RaycastHit>.Shared.Rent(2);

            for (int i = 1; i <= DETAIL; i++)
            {
                int count = Physics.RaycastNonAlloc(castStartPosition, down, raycastHitBuffer, groundCheckDistance, 1 << PhysicsConstants.GroundLayer);
                if (count > 0)
                {
                    var hit = raycastHitBuffer[0];
                    var hitPoint = hit.point;
                    transform.position = hitPoint + -down * (transform.localScale.y * 0.5f);
                    ArrayPool<RaycastHit>.Shared.Return(raycastHitBuffer);
                    return true;
                }

                var time = i / (float)DETAIL;
                castStartPosition = Vector3.Lerp(previousPosition, currentPosition, time) + down * localScaleYHalf;
            }

            ArrayPool<RaycastHit>.Shared.Return(raycastHitBuffer);
            return false;
        }
    }
}