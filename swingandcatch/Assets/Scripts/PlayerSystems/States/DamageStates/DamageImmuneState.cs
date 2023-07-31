using TheGame.FSM;
using UnityEngine;
using XIV.Core.Utils;
using XIV.Core.TweenSystem;

namespace TheGame.PlayerSystems.States.DamageStates
{
    public class DamageImmuneState : State<PlayerFSM, PlayerStateFactory>
    {
        const int COLOR_FLASH_COUNT = 2;
        Renderer[] playerRenderers;

        public DamageImmuneState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            float colorFlashDuration = 1f / (COLOR_FLASH_COUNT + 1);
            
            stateMachine.CancelTween();
            playerRenderers = stateMachine.GetComponentsInChildren<Renderer>();
            EasingFunction.Function easing = EasingFunction.EaseInOutCirc;
            for (int i = 0; i < playerRenderers.Length; i++)
            {
                var renderer = playerRenderers[i];
                renderer.CancelTween();
                renderer.XIVTween()
                    .RendererColor(renderer.material.color, Color.red, colorFlashDuration, easing, true, COLOR_FLASH_COUNT)
                    .Start();
            }
            stateMachine.XIVTween()
                .ScaleX(1f, 0.75f, colorFlashDuration, easing, true, COLOR_FLASH_COUNT)
                .Start();
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.damageable.CanReceiveDamage())
            {
                ChangeChildState(previousState);
            }
        }
    }
}