using TheGame.FSM;
using TheGame.HealthSystems;

namespace TheGame.PlayerSystems.States.DamageStates
{
    public class CheckDamageState : State<PlayerFSM, PlayerStateFactory>, IHealthListener
    {
        bool isDead;
        
        public CheckDamageState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom) => stateMachine.health.AddListener(this);
        protected override void OnStateExit() => stateMachine.health.RemoveListener(this);

        protected override void CheckTransitions()
        {
            if (isDead)
            {
                // var lastCollider = buffer[count - 1]; // last collider that damages the player
                ChangeRootState(factory.GetState<PlayerDiedByLavaState>());
                return;
            }

            if (stateMachine.damageImmune)
            {
                ChangeChildState(factory.GetState<DamageImmuneState>());
                return;
            }
            
        }

        public void OnHealthChanged(ref HealthChange _) => stateMachine.damageImmune = true;
        public void OnHealthDepleted(ref HealthChange _) => isDead = true;
    }
}