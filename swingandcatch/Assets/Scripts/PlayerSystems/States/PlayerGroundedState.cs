using System.Buffers;
using TheGame.AbilitySystems.Abilities;
using TheGame.AbilitySystems.Core;
using TheGame.FSM;
using TheGame.PlayerSystems.States.DamageStates;
using TheGame.Scripts.InputSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using XIV.Core.Extensions;

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
            stateMachine.playerVisualTransform.gameObject.SetActive(true);
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
            AddChildState(factory.GetState<PlayerCheckInteractableState>());
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.movementHandler.CheckIsTouching(1 << PhysicsConstants.GroundLayer) == false)
            {
                ChangeRootState(factory.GetState<PlayerFallingState>());
                return;
            }
            
            if (jumpPressed)
            {
                ChangeRootState(factory.GetState<PlayerJumpState>());
                return;
            }

            if (stateMachine.isReachedEndGate)
            {
                ChangeRootState(factory.GetState<PlayerWinState>());
                return;
            }

            // TODO : Replace the input with new Input System
            if (Input.GetKey(KeyCode.E))
            {
                var dashAbility = stateMachine.abilityHandler.GetAbility<DashAbility>();
                if (dashAbility != null && ((IAbility)dashAbility).IsAvailableToUse())
                {
                    stateMachine.abilityHandler.UseAbility(dashAbility);
                    stateMachine.playerVisualTransform.gameObject.SetActive(false);
                    ChangeRootState(factory.GetState<PlayerAbilityDrivenState>());
                }

                return;
            }

            // TODO : Replace the input with new Input System
            if (Input.GetKey(KeyCode.F))
            {
                var interactable = factory.GetState<PlayerCheckInteractableState>().interactable;
                if (interactable != default)
                {
                    var interactionState = factory.GetState<PlayerInteractionState>();
                    ChangeRootState(interactionState);
                    interactable.Interact(interactionState);
                }

                return;
            }
        }

        bool SetGroundedPosition()
        {
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int hitCount = stateMachine.movementHandler.CheckIsTouchingNonAlloc(buffer, 1 << PhysicsConstants.GroundLayer);

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
                stateMachine.movementHandler.SyncPosition();
            }
            
            ArrayPool<Collider2D>.Shared.Return(buffer);
            return hitCount > 0;
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