using TheGame.AbilitySystems;
using TheGame.FSM;
using TheGame.PlayerSystems.States.DamageStates;
using TheGame.Scripts.InputSystems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheGame.PlayerSystems.States
{
    public class OnAirMovementState : State<PlayerFSM, PlayerStateFactory>, DefaultGameInputs.IPlayerAirMovementActions
    {
        bool hasInput;
        float movementInput;
        
        public OnAirMovementState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
            InputManager.Inputs.PlayerAirMovement.SetCallbacks(this);
        }

        protected override void OnStateEnter(State comingFrom)
        {
            InputManager.Inputs.PlayerAirMovement.Enable();
        }

        protected override void OnStateUpdate()
        {
            if (hasInput == false) return;
            Transform transform = stateMachine.transform;
            Vector3 pos = transform.position;
            var dir = Vector3.right * movementInput;
            pos += dir * (stateMachine.stateDatas.airMovementStateDataSO.airMovementSpeed * Time.deltaTime);
            stateMachine.movementHandler.Move(pos);
            stateMachine.rotationHandler.LookDirection(dir);
        }

        protected override void OnStateExit()
        {
            InputManager.Inputs.PlayerAirMovement.Disable();
        }

        protected override void InitializeChildStates()
        {
            AddChildState(factory.GetState<CheckDamageState>());
            AddChildState(factory.GetState<PlayerAttackState>());
        }

        public void OnHorizontalMovement(InputAction.CallbackContext context)
        {
            hasInput = context.performed;
            movementInput = context.ReadValue<float>();
        }
    }
}