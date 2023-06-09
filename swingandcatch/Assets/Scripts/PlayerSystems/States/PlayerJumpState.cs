﻿using TheGame.FSM;
using TheGame.Scripts.InputSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using XIV.Core.Utils;
using XIV.Core.TweenSystem;

namespace TheGame.PlayerSystems.States
{
    public class PlayerJumpState : State<PlayerFSM, PlayerStateFactory>
    {
        Timer waitGroundedTimer = new Timer(0.25f);
        float yVelocity;
        bool hasAirMovementInput;
        
        public PlayerJumpState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            RegisterInputActions();
            
            yVelocity = CalculateJumpVelocity(stateMachine.stateDatas.jumpStateDataSO.jumpHeight);

            if (comingFrom is PlayerClimbState) return;
            stateMachine.playerVisualTransform.CancelTween();
            stateMachine.playerVisualTransform.XIVTween()
                .ScaleX(stateMachine.playerVisualTransform.localScale.x, 0.75f, 0.25f, EasingFunction.EaseInOutBounce, true)
                .Start();
        }

        protected override void OnStateUpdate()
        {
            var dt = Time.fixedDeltaTime * Time.timeScale;
            
            yVelocity += Physics.gravity.y * (stateMachine.stateDatas.jumpStateDataSO.jumpGravityScale * dt);
            var pos = stateMachine.transform.position;
            pos.y += yVelocity * dt;
            
            if (hasAirMovementInput) stateMachine.SyncPosition();

            if (stateMachine.Move(pos) == false) yVelocity = 0f;
        }

        protected override void OnStateExit()
        {
            UnregisterInputActions();
            
            waitGroundedTimer.Restart();
            stateMachine.playerVisualTransform.CancelTween();
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.GetNearestRope(out var rope) && yVelocity < 0.25f && rope.GetClosestPoint(stateMachine.transform.position).index != 0)
            {
                ChangeRootState(factory.GetState<PlayerClimbState>());
                return;
            }
            
            if (stateMachine.CheckIsTouching(1 << PhysicsConstants.GroundLayer) && waitGroundedTimer.Update(Time.deltaTime))
            {
                ChangeRootState(factory.GetState<PlayerGroundedState>());
                return;
            }
            
            if (yVelocity < 0f)
            {
                ChangeRootState(factory.GetState<PlayerFallingState>());
                return;
            }
        }

        protected override void InitializeChildStates()
        {
            AddChildState(factory.GetState<OnAirMovementState>());
        }

        float CalculateJumpVelocity(float jumpHeight)
        {
            float gravity = Physics.gravity.y * stateMachine.stateDatas.jumpStateDataSO.jumpGravityScale;
            float initialVelocity = Mathf.Sqrt(2f * jumpHeight * -gravity);
            return initialVelocity;
        }

        void RegisterInputActions()
        {
            InputManager.Inputs.PlayerAirMovement.HorizontalMovement.started += OnMovementPress;
            InputManager.Inputs.PlayerAirMovement.HorizontalMovement.canceled += OnMovementPress;
        }

        void UnregisterInputActions()
        {
            InputManager.Inputs.PlayerAirMovement.HorizontalMovement.started -= OnMovementPress;
            InputManager.Inputs.PlayerAirMovement.HorizontalMovement.canceled -= OnMovementPress;
        }

        void OnMovementPress(InputAction.CallbackContext context)
        {
            hasAirMovementInput = false;
        }
    }
}