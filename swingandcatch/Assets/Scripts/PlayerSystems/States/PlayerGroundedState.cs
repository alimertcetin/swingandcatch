using System.Buffers;
using TheGame.FSM;
using TheGame.PlayerSystems.States.DamageStates;
using UnityEngine;
using XIV.Core;
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
            var bottomPosition = position + -stateMachineTransform.up * (localScale.y * 0.5f);
            
            var buffer = ArrayPool<Collider>.Shared.Rent(2);
            int hitCount = Physics.OverlapBoxNonAlloc(position, localScale * 0.5f, buffer, stateMachineTransform.rotation, 1 << PhysicsConstants.GroundLayer);

            if (hitCount > 0)
            {
                Collider closestCollider = default;
                float distance = float.MaxValue;
                for (int i = 0; i < hitCount; i++)
                {
                    var posOnCol = buffer[i].ClosestPoint(bottomPosition);
                    var dis = Vector3.Distance(posOnCol, bottomPosition);
                    if (dis < distance)
                    {
                        distance = dis;
                        closestCollider = buffer[i];
                    }
                    XIV.Core.XIVDebug.DrawCircle(posOnCol, 0.1f, Color.red, 1.5f);
                }
                
                var y = closestCollider.transform.position.y + closestCollider.bounds.extents.y;
                var closestPointOnCollider = new Vector3(position.x, y, position.z);
                var groundedPos = closestPointOnCollider + stateMachineTransform.up * (localScale.y * 0.5f);
                
                XIV.Core.XIVDebug.DrawCircle(groundedPos, 0.1f, Color.green, 1.5f);
                
                stateMachineTransform.position = groundedPos;

            }
            
            ArrayPool<Collider>.Shared.Return(buffer);
            return hitCount > 0;
        }
    }
}