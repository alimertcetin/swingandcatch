using TheGame.FSM;
using TheGame.PlayerSystems.States;

namespace TheGame.PlayerSystems
{
    public class PlayerStateFactory : StateFactory<PlayerFSM>
    {
        public PlayerStateFactory(PlayerFSM stateMachine) : base(stateMachine)
        {
            AddState(new PlayerGroundedState(stateMachine, this));
            AddState(new PlayerIdleState(stateMachine, this));
            AddState(new PlayerWalkState(stateMachine, this));
            AddState(new PlayerRunSate(stateMachine, this));
            AddState(new PlayerFallingState(stateMachine, this));
            AddState(new PlayerJumpState(stateMachine, this));
            AddState(new OnAirMovementState(stateMachine, this));
            AddState(new PlayerClimbState(stateMachine, this));
        }
    }
}