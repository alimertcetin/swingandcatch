using TheGame.FSM;
using UnityEngine;
using XIV.Core.Extensions;

namespace TheGame.PlayerSystems.States.AnimationStates
{
    public class PlayerBreathAnimationState : State<PlayerFSM, PlayerStateFactory>
    {
        public float animationDuration;
        float timePassed;
        float timePassedSinceEnter;
        
        public PlayerBreathAnimationState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            timePassed = 0f;
            timePassedSinceEnter = 0f;
        }

        protected override void OnStateUpdate()
        {
            timePassedSinceEnter += Time.deltaTime;
            if (timePassedSinceEnter < 1f) return;
            
            timePassed += Time.deltaTime;
            
            var t = timePassed / animationDuration;
            
            var startScale = t > 0.5f ? Vector3.one * 0.8f : Vector3.one;
            var targetScale = t > 0.5f ? Vector3.one : Vector3.one * 0.8f;
            var newScale = Vector3.Lerp(startScale, targetScale, t);
            stateMachine.transform.localScale = newScale.SetY(stateMachine.transform.localScale.y);
            
            if (timePassed > animationDuration)
            {
                timePassed = 0f;
            }
        }

        protected override void OnStateExit()
        {
            stateMachine.transform.localScale = Vector3.one;
        }
        
    }
}