using TheGame.FSM;

namespace TheGame.PlayerSystems.States.DamageStates
{
    public class CheckDamageState : State<PlayerFSM, PlayerStateFactory>
    {
        public CheckDamageState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.damageable.GetHealth().isDepleted)
            {
                ChangeRootState(factory.GetState<PlayerDiedState>());
                return;
            }

            if (stateMachine.damageable.CanReceiveDamage() == false)
            {
                ChangeChildState(factory.GetState<DamageImmuneState>());
                return;
            }
            
        }
    }
}