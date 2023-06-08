using TheGame.FSM;

namespace TheGame.PlayerSystems.States
{
    public class PlayerIdleState : State<PlayerFSM, PlayerStateFactory>
    {
        public PlayerIdleState(PlayerFSM stateMachine, PlayerStateFactory factory) : base(stateMachine, factory)
        {
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.hasMovementInput == false) return;
            
            if (stateMachine.isRunPressed)
            {
                ChangeChildState(factory.GetState<PlayerRunSate>());
                return;
            }

            ChangeChildState(factory.GetState<PlayerWalkState>());
        }
    }
}