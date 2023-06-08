using TheGame.FSM;
using UnityEngine;

namespace TheGame.PlayerSystems.States
{
    public class PlayerRunSate : State<PlayerFSM, PlayerStateFactory>
    {
        public PlayerRunSate(PlayerFSM stateMachine, PlayerStateFactory factory) : base(stateMachine, factory)
        {
        }

        protected override void OnStateUpdate()
        {
            stateMachine.transform.position += stateMachine.movementInput.normalized * (stateMachine.runSpeed * Time.deltaTime);
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.isRunPressed == false && stateMachine.hasMovementInput)
            {
                ChangeChildState(factory.GetState<PlayerWalkState>());
                return;
            }

            if (stateMachine.hasMovementInput == false)
            {
                ChangeChildState(factory.GetState<PlayerIdleState>());
                return;
            }
        }
    }
}