using TheGame.FSM;
using TheGame.PlayerSystems.States;
using TheGame.PlayerSystems.States.AnimationStates;
using TheGame.PlayerSystems.States.DamageStates;

namespace TheGame.PlayerSystems
{
    public class PlayerStateFactory : StateFactory<PlayerFSM>
    {
        public PlayerStateFactory(PlayerFSM stateMachine) : base(stateMachine)
        {
            AddState(new PlayerGroundedState(stateMachine, this));
            AddState(new PlayerIdleState(stateMachine, this));
            AddState(new PlayerWalkState(stateMachine, this));
            AddState(new PlayerRunState(stateMachine, this));
            AddState(new PlayerFallingState(stateMachine, this));
            AddState(new PlayerJumpState(stateMachine, this));
            AddState(new OnAirMovementState(stateMachine, this));
            AddState(new PlayerClimbState(stateMachine, this));
            AddState(new PlayerFeetMovementAnimationState(stateMachine, this));
            AddState(new PlayerBreathAnimationState(stateMachine, this));
            AddState(new PlayerDiedByLavaState(stateMachine, this));
            AddState(new CheckDamageState(stateMachine, this));
            AddState(new DamageImmuneState(stateMachine, this));
            AddState(new PlayerWinState(stateMachine, this));
            AddState(new PlayerAttackState(stateMachine, this));
            
            AddState(new PlayerAbilityDrivenState(stateMachine, this));
        }
    }
}