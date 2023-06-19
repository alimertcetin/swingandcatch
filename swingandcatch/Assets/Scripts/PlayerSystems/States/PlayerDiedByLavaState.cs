using TheGame.FSM;
using XIV.Core.Utils;
using XIV.Core.TweenSystem;
using XIV.EventSystem;
using XIV.EventSystem.Events;

namespace TheGame.PlayerSystems.States
{
    public class PlayerDiedByLavaState : State<PlayerFSM, PlayerStateFactory>
    {
        public PlayerDiedByLavaState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            var pos = stateMachine.transform.position;
            stateMachine.CancelTween();
            stateMachine.XIVTween()
                .MoveY(pos.y, pos.y + 2f, 1f, EasingFunction.Linear, true)
                .Start();
            XIVEventSystem.SendEvent(new InvokeAfterEvent(1.2f).OnCompleted(() =>
            {
                stateMachine.playerDiedChannelSO.RaiseEvent(stateMachine.transform);
            }));
        }
    }
}