using TheGame.FSM;
using XIV.Core.Utils;
using XIV.Core.TweenSystem;

namespace TheGame.PlayerSystems.States
{
    public class PlayerDiedState : State<PlayerFSM, PlayerStateFactory>
    {
        public PlayerDiedState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            var pos = stateMachine.transform.position;
            stateMachine.CancelTween();
            stateMachine.XIVTween()
                .MoveY(pos.y, pos.y + 2f, 1f, EasingFunction.Linear, true)
                .Start();
        }
    }
}