using TheGame.EnemySystems.SawBlade.States;
using TheGame.FSM;

namespace TheGame.EnemySystems.SawBlade
{
    public class SawBladeStateFactory : StateFactory<SawBladeFSM>
    {
        public SawBladeStateFactory(SawBladeFSM stateMachine) : base(stateMachine)
        {
            AddState(new SawBladeIdleTransitionState(stateMachine, this));
            AddState(new SawBladeIdleState(stateMachine, this));
            AddState(new SawBladeAttackState(stateMachine, this));
        }
    }
}
