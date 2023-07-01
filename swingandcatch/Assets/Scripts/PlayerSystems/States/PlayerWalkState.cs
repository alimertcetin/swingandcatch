using TheGame.FSM;
using TheGame.PlayerSystems.States.AnimationStates;
using TheGame.Scripts.InputSystems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheGame.PlayerSystems.States
{
    public class PlayerWalkState : State<PlayerFSM, PlayerStateFactory>
    {
        const float FEET_ANIMATION_DURATION = 0.15f;
        bool movementPressed;
        float horizonalMovementInput;
        
        public PlayerWalkState(PlayerFSM stateMachine, PlayerStateFactory factory) : base(stateMachine, factory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            RegisterInputActions();
            horizonalMovementInput = InputManager.Inputs.PlayerGrounded.HorizontalMovement.ReadValue<float>();
            movementPressed = true;
        }

        protected override void OnStateUpdate()
        {
            var pos = stateMachine.transform.position;
            pos += Vector3.right * (horizonalMovementInput * (stateMachine.stateDatas.walkStateDataSO.walkSpeed * Time.deltaTime));
            stateMachine.Move(pos);
        }

        protected override void OnStateExit()
        {
            UnregisterInputActions();
        }

        protected override void InitializeChildStates()
        {
            var feetAnimationState = factory.GetState<PlayerFeetMovementAnimationState>();
            feetAnimationState.animationTime = FEET_ANIMATION_DURATION;
            AddChildState(feetAnimationState);
        }

        protected override void CheckTransitions()
        {
            if (InputManager.Inputs.PlayerGrounded.Run.IsPressed())
            {
                ChangeChildState(factory.GetState<PlayerRunSate>());
                return;
            }
            
            if (movementPressed == false)
            {
                ChangeChildState(factory.GetState<PlayerIdleState>());
                return;
            }
        }

        void RegisterInputActions()
        {
            InputManager.Inputs.PlayerGrounded.HorizontalMovement.performed += OnGroundedMovementPerformed;
            InputManager.Inputs.PlayerGrounded.HorizontalMovement.canceled += OnGroundedMovementCanceled;
        }

        void UnregisterInputActions()
        {
            InputManager.Inputs.PlayerGrounded.HorizontalMovement.performed -= OnGroundedMovementPerformed;
            InputManager.Inputs.PlayerGrounded.HorizontalMovement.canceled += OnGroundedMovementCanceled;
        }

        void OnGroundedMovementPerformed(InputAction.CallbackContext context)
        {
            movementPressed = true;
            horizonalMovementInput = context.ReadValue<float>();
        }

        void OnGroundedMovementCanceled(InputAction.CallbackContext context)
        {
            movementPressed = false;
        }
    }
}