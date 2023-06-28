using System.Buffers;
using TheGame.FSM;
using TheGame.PlayerSystems.States.DamageStates;
using UnityEngine;
using XIV.Core.Extensions;
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
            var stateMachineTransform = stateMachine.transform;
            var position = stateMachineTransform.position;
            var localScale = stateMachineTransform.localScale;
            
            var buffer = ArrayPool<Collider>.Shared.Rent(2);
            int hitCount = Physics.OverlapBoxNonAlloc(position, localScale * 0.5f, buffer, stateMachineTransform.rotation, 1 << PhysicsConstants.GroundLayer);

            if (hitCount > 0)
            {
                var closestCollider = buffer.GetClosest(position, hitCount);
                var closestPointOnCollider = closestCollider.ClosestPoint(position);
                var groundedPos = closestPointOnCollider + stateMachineTransform.up * (localScale.y * 0.5f);
                stateMachineTransform.position = groundedPos;
            }
            
            ArrayPool<Collider>.Shared.Return(buffer);
            return hitCount > 0;
        }
    }
}