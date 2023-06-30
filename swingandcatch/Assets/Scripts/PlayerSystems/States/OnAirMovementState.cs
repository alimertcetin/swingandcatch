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
            if (stateMachine.hasHorizontalMovementInput == false) return;
            Transform transform = stateMachine.transform;
            Vector3 pos = transform.position;
            pos += stateMachine.horizontalMovementInput.normalized * (stateMachine.stateDatas.airMovementStateDataSO.airMovementSpeed * Time.deltaTime);
            stateMachine.Move(pos);
        }

        protected override void InitializeChildStates()
        {
            AddChildState(factory.GetState<CheckDamageState>());
        }
    }
}