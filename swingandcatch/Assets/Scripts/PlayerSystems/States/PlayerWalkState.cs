using TheGame.FSM;
using UnityEngine;

namespace TheGame.PlayerSystems.States
{
    public class PlayerWalkState : State<PlayerFSM, PlayerStateFactory>
    {
        public PlayerWalkState(PlayerFSM stateMachine, PlayerStateFactory factory) : base(stateMachine, factory)
        {
        }

        protected override void OnStateUpdate()
        {
            stateMachine.transform.position += stateMachine.movementInput.normalized * (stateMachine.walkSpeed * Time.deltaTime);
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.isRunPressed && stateMachine.hasMovementInput)
            {
                ChangeChildState(factory.GetState<PlayerRunSate>());
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