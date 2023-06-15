using TheGame.FSM;
using UnityEngine;
using XIV.Core.Utils;
using XIV.TweenSystem;

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
            float colorFlashDuration = (stateMachine.damageImmuneDuration / (COLOR_FLASH_COUNT + 1));
            timer = new Timer(colorFlashDuration);
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
                .Scale(Vector3.one, Vector3.one * 0.75f, colorFlashDuration, easing, true, COLOR_FLASH_COUNT)
                .Start();
        }

        protected override void OnStateUpdate()
        {
            timer.Update(Time.deltaTime);
        }

        protected override void CheckTransitions()
        {
            if (timer.IsDone)
            {
                Debug.Log("timePassed = " + timer.PassedTime);
                ChangeChildState(previousState);
            }
        }
    }
}