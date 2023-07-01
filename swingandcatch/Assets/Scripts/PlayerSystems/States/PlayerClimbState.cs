using System.Collections.Generic;
using TheGame.FSM;
using TheGame.PlayerSystems.States.DamageStates;
using TheGame.Scripts.InputSystems;
using TheGame.VerletRope;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using XIV.Core;
using XIV.Core.Utils;
using XIV.Core.TweenSystem;
using XIV.Core.XIVMath;

namespace TheGame.PlayerSystems.States
{
    public class PlayerClimbState : State<PlayerFSM, PlayerStateFactory>, DefaultGameInputs.IPlayerClimbActions
    {
        Rope currentRope;
        float currentT;
        List<Vector3> positionBuffer;
        float horizontalMovementInput;
        float verticalMovementInput;
        bool hasVerticalInput;
        bool hasHorizontalInput;
        bool isJumpPressed;

        public PlayerClimbState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
            InputManager.Inputs.PlayerClimb.SetCallbacks(this);
        }

        protected override void OnStateEnter(State comingFrom)
        {
            InputManager.Inputs.PlayerClimb.Enable();
            
            stateMachine.GetNearestRope(out currentRope);
            positionBuffer = ListPool<Vector3>.Get();
            
            Vector3 pos = stateMachine.transform.position;

            currentRope.GetPositionsNonAlloc(positionBuffer);
            currentT = Mathf.Clamp01(SplineMath.GetTime(pos, positionBuffer));
            
            Vector3 positionOnSegment = SplineMath.GetPoint(positionBuffer, currentT);
            stateMachine.transform.position = positionOnSegment;
            
            currentRope.AddForce(positionOnSegment, stateMachine.velocity.normalized * stateMachine.stateDatas.climbStateDataSO.ropeSwingInitialForce);
            
            stateMachine.playerVisualTransform.CancelTween();
            stateMachine.playerVisualTransform.XIVTween()
                .Scale(stateMachine.playerVisualTransform.localScale, Vector3.one * 1.25f, 0.15f, EasingFunction.EaseInOutBounce, true)
                .Start();
        }

        protected override void OnStateFixedUpdate()
        {
            positionBuffer.Clear();
            currentRope.GetPositionsNonAlloc(positionBuffer);

            Vector3 pos;
            if (hasVerticalInput == false)
            {
                var currentPosition = SplineMath.GetPoint(positionBuffer, currentT);
                stateMachine.transform.position = currentPosition;
                pos = currentPosition;
            }
            else
            {
                currentT -= stateMachine.stateDatas.climbStateDataSO.climbSpeed * Time.deltaTime * verticalMovementInput;
                currentT = Mathf.Clamp01(currentT);
                var nextPosition = SplineMath.GetPoint(positionBuffer, currentT);
                stateMachine.transform.position = nextPosition;
                pos = nextPosition;
            }


            if (hasHorizontalInput)
            {
                currentRope.AddForce(pos, Vector3.right * (horizontalMovementInput * stateMachine.stateDatas.climbStateDataSO.ropeSwingForce));
            }
        }

        protected override void OnStateExit()
        {
            InputManager.Inputs.PlayerClimb.Disable();
            
            ListPool<Vector3>.Release(positionBuffer);
            stateMachine.CancelTween();
        }

        protected override void InitializeChildStates()
        {
            AddChildState(factory.GetState<CheckDamageState>());
        }

        protected override void CheckTransitions()
        {
            if (isJumpPressed)
            {
                ChangeRootState(factory.GetState<PlayerJumpState>());
                return;
            }
        }
        
        void DefaultGameInputs.IPlayerClimbActions.OnVerticalMovement(InputAction.CallbackContext context)
        {
            hasVerticalInput = context.performed;
            verticalMovementInput = context.ReadValue<float>();
        }

        void DefaultGameInputs.IPlayerClimbActions.OnHorizontalMovement(InputAction.CallbackContext context)
        {
            hasHorizontalInput = context.performed;
            horizontalMovementInput = context.ReadValue<float>();
        }

        void DefaultGameInputs.IPlayerClimbActions.OnJumpTransition(InputAction.CallbackContext context)
        {
            isJumpPressed = context.performed;
        }
    }
}