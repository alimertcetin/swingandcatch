using TheGame.FSM;
using UnityEngine;
using XIV.Core.Utils;
using XIV.Core.TweenSystem;

namespace TheGame.PlayerSystems.States
{
    public class PlayerWinState : State<PlayerFSM, PlayerStateFactory>
    {
        public PlayerWinState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            stateMachine.CancelTween();
            var renderer = stateMachine.GetComponentsInChildren<Renderer>()[0];
            stateMachine.XIVTween()
                .Scale(Vector3.one, Vector3.one * 0.5f, 0.5f, EasingFunction.EaseInOutBounce, true, 1)
                .Wait(0.25f)
                .OnComplete(() =>
                {
                    stateMachine.playerReachedEndChannelSO.RaiseEvent(stateMachine.transform);
                })
                .Start();
            renderer.XIVTween()
                .RendererColor(renderer.material.color, Color.green, 0.5f, EasingFunction.Linear, true, 1)
                .Start();
        }
    }
}