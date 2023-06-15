using TheGame.FSM;
using TheGame.PlayerSystems.States.DamageStates;
using UnityEngine;

namespace TheGame.PlayerSystems.States
{
    public class OnAirMovementState : State<PlayerFSM, PlayerStateFactory>
    {
        public OnAirMovementState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateUpdate()
        {
            Transform transform = stateMachine.transform;
            Vector3 pos = transform.position;
            pos += stateMachine.movementInput.normalized * (stateMachine.airMovementSpeed * Time.deltaTime);
            transform.position = pos;
        }

        protected override void InitializeChildStates()
        {
            AddChildState(factory.GetState<CheckDamageState>());
        }
    }
}