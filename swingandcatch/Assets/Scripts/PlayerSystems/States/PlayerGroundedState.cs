using System.Buffers;
using TheGame.FSM;
using TheGame.PlayerSystems.States.DamageStates;
using TheGame.Scripts.InputSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using XIV.Core.Extensions;
using XIV.EventSystem;
using XIV.EventSystem.Events;

namespace TheGame.PlayerSystems.States
{
    public class PlayerGroundedState : State<PlayerFSM, PlayerStateFactory>
    {
        bool jumpPressed;
        
        public PlayerGroundedState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            InputManager.Inputs.PlayerGrounded.Enable();
            InputManager.Inputs.PlayerGrounded.JumpTransition.performed += OnGroundedJumpTransition;
            
            if (SetGroundedPosition()) return;
#if UNITY_EDITOR
            if (FSMDebugSettings.IsStateChangeLogsEnabled) Debug.LogError("Player is not grounded but still made a transition to the GroundedState from " + comingFrom.GetType().Name);
#endif
        }

        protected override void OnStateExit()
        {
            InputManager.Inputs.PlayerGrounded.JumpTransition.performed -= OnGroundedJumpTransition;
            InputManager.Inputs.PlayerGrounded.Disable();
        }

        protected override void InitializeChildStates()
        {
            AddChildState(factory.GetState<PlayerIdleState>());
            AddChildState(factory.GetState<CheckDamageState>());
            AddChildState(factory.GetState<PlayerAttackState>());
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.CheckIsTouching(1 << PhysicsConstants.GroundLayer) == false)
            {
                ChangeRootState(factory.GetState<PlayerFallingState>());
                return;
            }
            
            if (jumpPressed)
            {
                jumpPressed = false;
                ChangeRootState(factory.GetState<PlayerJumpState>());
                return;
            }
            
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int count = Physics2D.OverlapCircleNonAlloc(stateMachine.transform.position, 0.5f, buffer, 1 << PhysicsConstants.EndGateLayer);
            ArrayPool<Collider2D>.Shared.Return(buffer);
            
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
                return;
            }
        }

        void OnGroundedJumpTransition(InputAction.CallbackContext context)
        {
            jumpPressed = context.performed;
        }

        bool SetGroundedPosition()
        {
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int hitCount = stateMachine.CheckIsTouchingNonAlloc(buffer, 1 << PhysicsConstants.GroundLayer);

            if (hitCount > 0)
            {
                var stateMachineTransformPosition = stateMachine.transform.position;
                var bottomColliderSize = stateMachine.bottomColliderSize;
                var bottomColliderPosLocal = stateMachine.bottomColliderPosLocal;
                var colliderCircleRadius = bottomColliderSize.y * 0.5f;
                
                var bottomMiddleLocalPosition = bottomColliderPosLocal + Vector3.down * (colliderCircleRadius);
                var bottomMiddleWorldPosition = stateMachineTransformPosition + bottomMiddleLocalPosition;
                var closestCollider = GetClosest(bottomMiddleWorldPosition, buffer, hitCount, out var positionOnCollider);

                var targetY = closestCollider.transform.position.y + closestCollider.bounds.extents.y;
                var xDistanceToColliderPos = positionOnCollider.x - bottomMiddleWorldPosition.x;
                var xDistanceToCircleCenter = bottomMiddleLocalPosition.x + ((bottomColliderSize.x * 0.5f) - colliderCircleRadius);
                var diff = Mathf.Abs(xDistanceToColliderPos) - xDistanceToCircleCenter;

                var groundedPos = stateMachineTransformPosition.SetY(targetY);
                if (Mathf.Abs(diff) < colliderCircleRadius)
                {
                    // TODO : Create a PlayerSlidingState?
                    groundedPos.x += xDistanceToColliderPos > 0 ? -diff : diff;
                }

                stateMachine.transform.position = (groundedPos - bottomMiddleLocalPosition).SetZ(stateMachineTransformPosition.z);
                stateMachine.SyncPosition();
            }
            
            ArrayPool<Collider2D>.Shared.Return(buffer);
            return hitCount > 0;
        }

        public static Collider2D GetClosest(Vector3 bottomPosition, Collider2D[] buffer, int hitCount, out Vector3 positionOnCollider)
        {
            Collider2D closestCollider = default;
            positionOnCollider = default;
            float distance = float.MaxValue;
            for (int i = 0; i < hitCount; i++)
            {
                positionOnCollider = buffer[i].ClosestPoint(bottomPosition);
                var dis = Vector3.Distance(positionOnCollider, bottomPosition);
                if (dis < distance)
                {
                    distance = dis;
                    closestCollider = buffer[i];
                }
                XIV.Core.XIVDebug.DrawCircle(positionOnCollider, 0.1f, Color.red, 1.5f);
            }

            return closestCollider;
        }
    }
}