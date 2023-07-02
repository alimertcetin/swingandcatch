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
    public class PlayerGroundedState : State<PlayerFSM, PlayerStateFactory>, DefaultGameInputs.IPlayerGroundedActions
    {
        public bool runPressed { get; private set; }
        public bool jumpPressed { get; private set; }
        public bool horizontalMovementPressed { get; private set; }
        public float horizontalMovementInput { get; private set; }
        
        public PlayerGroundedState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
            InputManager.Inputs.PlayerGrounded.SetCallbacks(this);
        }

        protected override void OnStateEnter(State comingFrom)
        {
            InputManager.Inputs.PlayerGrounded.Enable();
            
            if (SetGroundedPosition()) return;
#if UNITY_EDITOR
            if (FSMDebugSettings.IsStateChangeLogsEnabled) Debug.LogError("Player is not grounded but still made a transition to the GroundedState from " + comingFrom.GetType().Name);
#endif
        }

        protected override void OnStateExit()
        {
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
                ChangeRootState(factory.GetState<PlayerJumpState>());
                return;
            }

            if (CheckWinStateTransition())
            {
                return;
            }
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
                var closestCollider = buffer.GetClosestCollider(bottomMiddleWorldPosition, hitCount, out var positionOnCollider);

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

        bool CheckWinStateTransition()
        {
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int count = Physics2D.OverlapCircleNonAlloc(stateMachine.transform.position, 0.5f, buffer, 1 << PhysicsConstants.EndGateLayer);
            ArrayPool<Collider2D>.Shared.Return(buffer);

            if (count == 0) return false;
            
            var animator = buffer[0].GetComponentInChildren<Animator>();
            animator.Play(AnimationConstants.EndGate.Clips.EndGate_Open);
            XIVEventSystem.SendEvent(new InvokeAfterEvent(1.5f).OnCompleted(() => { animator.Play(AnimationConstants.EndGate.Clips.EndGate_Close); }));
            var winState = factory.GetState<PlayerWinState>();
            winState.endGatePosition = buffer[0].transform.position;
            return true;
        }

        void DefaultGameInputs.IPlayerGroundedActions.OnHorizontalMovement(InputAction.CallbackContext context)
        {
            if (context.performed) horizontalMovementPressed = true;
            else if (context.canceled) horizontalMovementPressed = false;
            horizontalMovementInput = context.ReadValue<float>();
        }

        void DefaultGameInputs.IPlayerGroundedActions.OnRun(InputAction.CallbackContext context)
        {
            if (context.performed) runPressed = true;
            else if (context.canceled) runPressed = false;
        }

        void DefaultGameInputs.IPlayerGroundedActions.OnJumpTransition(InputAction.CallbackContext context)
        {
            if (context.performed) jumpPressed = true;
            else if (context.canceled) jumpPressed = false;
        }
    }
}