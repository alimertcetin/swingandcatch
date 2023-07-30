using TheGame.FSM;

namespace TheGame.PlayerSystems.States
{
    public class PlayerAbilityDrivenState : State<PlayerFSM, PlayerStateFactory>
    {
        public PlayerAbilityDrivenState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.abilityHandler.HasActiveAbility() == false)
            {
                ChangeRootState(previousState);
                return;
            }
        }
    }
}