using System.Buffers;
using TheGame.FSM;
using TheGame.PlayerSystems.States.DamageStates;
using UnityEngine;
using XIV.EventSystem;
using XIV.EventSystem.Events;

namespace TheGame.PlayerSystems.States
{
    public class PlayerGroundedState : State<PlayerFSM, PlayerStateFactory>
    {
        Collider2D[] buffer;
        
        public PlayerGroundedState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            if (SetGroundedPosition()) return;
#if UNITY_EDITOR
            if (FSMDebugSettings.IsStateChangeLogsEnabled) Debug.LogError("Player is not grounded but still made a transition to the GroundedState from " + comingFrom.GetType().Name);
#endif
        }

        protected override void OnStateExit()
        {
            ArrayPool<Collider2D>.Shared.Return(buffer);
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
            int count = Physics2D.OverlapCircleNonAlloc(stateMachine.transform.position, 0.5f, buffer, 1 << PhysicsConstants.EndGateLayer);
            if (count > 0)
            {
                var animator = buffer[0].GetComponentInChildren<Animator>();
                animator.Play(AnimationConstants.EndGate.Clips.EndGate_Open);
                XIVEventSystem.SendEvent(new InvokeAfterEvent(1.5f).OnCompleted(() =>
                {
                    animator.Play(AnimationConstants.EndGate.Clips.EndGate_Close);
                }));
                var winState = factory.GetState<PlayerWinState>();
                winState.endGatePosition = buffer[0].transform.position;
                ChangeRootState(winState);
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
            var groundCheckDistance = stateMachine.groundedStateDataSO.groundCheckDistance;

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