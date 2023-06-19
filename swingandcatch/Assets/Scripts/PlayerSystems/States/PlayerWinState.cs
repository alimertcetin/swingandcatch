using TheGame.FSM;
using UnityEngine;
using XIV.Core.Utils;
using XIV.TweenSystem;

namespace TheGame.PlayerSystems.States
{
    public class PlayerWinState : State<PlayerFSM, PlayerStateFactory>
    {
        public Vector3 endGatePosition;
        public PlayerWinState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            var pos = stateMachine.transform.position;
            var movePos = endGatePosition + Vector3.forward * 2.5f;
            stateMachine.CancelTween();
            var renderer = stateMachine.GetComponentsInChildren<Renderer>()[0];
            stateMachine.XIVTween()
                .Scale(Vector3.one, Vector3.one * 0.5f, 0.5f, EasingFunction.EaseInOutBounce, true, 1)
                .And()
                .Move(pos, endGatePosition, 0.5f, EasingFunction.Linear)
                .Move(endGatePosition, movePos, 0.5f, EasingFunction.Linear)
                .Wait(1.5f)
                .Move(movePos, movePos + Vector3.up * 0.5f, 0.2f, EasingFunction.Linear)
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