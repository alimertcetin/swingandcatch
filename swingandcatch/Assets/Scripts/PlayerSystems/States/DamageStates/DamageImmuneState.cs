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
        Timer timer;

        public DamageImmuneState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            stateMachine.damageHandler.SetImmuneState(true);

            var immuneDuration = stateMachine.damageHandler.GetImmuneDuration();
            
            float colorFlashDuration = (immuneDuration / (COLOR_FLASH_COUNT + 1));
            timer = new Timer(immuneDuration);
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

        protected override void OnStateUpdate()
        {
            timer.Update(Time.deltaTime);
        }

        protected override void OnStateExit()
        {
            stateMachine.damageHandler.SetImmuneState(false);
        }

        protected override void CheckTransitions()
        {
            if (timer.IsDone)
            {
                ChangeChildState(previousState);
            }
        }
    }
}