using TheGame.FSM;
using TheGame.PlayerSystems.States.AnimationStates;
using TheGame.Scripts.InputSystems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheGame.PlayerSystems.States
{
    public class PlayerRunSate : State<PlayerFSM, PlayerStateFactory>
    {
        const float FEET_ANIMATION_DURATION = 0.1f;
        bool runPressed;
        float movementInput;
        
        public PlayerRunSate(PlayerFSM stateMachine, PlayerStateFactory factory) : base(stateMachine, factory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            RegisterInputActions();
            runPressed = true;
            movementInput = InputManager.Inputs.PlayerGrounded.HorizontalMovement.ReadValue<float>();
        }

        protected override void OnStateUpdate()
        {
            var pos = stateMachine.transform.position;
            pos += Vector3.right * (movementInput * (stateMachine.stateDatas.runStateDataSO.runSpeed * Time.deltaTime));
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
            if (runPressed == false)
            {
                ChangeChildState(factory.GetState<PlayerWalkState>());
                return;
            }
        }

        void RegisterInputActions()
        {
            InputManager.Inputs.PlayerGrounded.HorizontalMovement.performed += OnGroundedHorizontalMovement;
            InputManager.Inputs.PlayerGrounded.HorizontalMovement.canceled += OnGroundedHorizontalMovement;
            InputManager.Inputs.PlayerGrounded.Run.canceled += OnGroundedRun;
        }

        void UnregisterInputActions()
        {
            InputManager.Inputs.PlayerGrounded.HorizontalMovement.performed -= OnGroundedHorizontalMovement;
            InputManager.Inputs.PlayerGrounded.HorizontalMovement.canceled -= OnGroundedHorizontalMovement;
            InputManager.Inputs.PlayerGrounded.Run.canceled -= OnGroundedRun;
        }

        void OnGroundedHorizontalMovement(InputAction.CallbackContext context)
        {
            movementInput = context.ReadValue<float>();
        }

        void OnGroundedRun(InputAction.CallbackContext context)
        {
            runPressed = false;
        }
    }
}