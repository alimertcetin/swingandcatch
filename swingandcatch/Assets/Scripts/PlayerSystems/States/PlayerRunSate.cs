using TheGame.FSM;
using TheGame.PlayerSystems.States.AnimationStates;
using UnityEngine;

namespace TheGame.PlayerSystems.States
{
    public class PlayerRunSate : State<PlayerFSM, PlayerStateFactory>
    {
        const float FEET_ANIMATION_DURATION = 0.1f;
        
        public PlayerRunSate(PlayerFSM stateMachine, PlayerStateFactory factory) : base(stateMachine, factory)
        {
        }

        protected override void OnStateUpdate()
        {
            stateMachine.transform.position += stateMachine.movementInput.normalized * (stateMachine.runSpeed * Time.deltaTime);
        }

        protected override void InitializeChildStates()
        {
            var feetAnimationState = factory.GetState<PlayerFeetMovementAnimationState>();
            feetAnimationState.animationTime = FEET_ANIMATION_DURATION;
            AddChildState(feetAnimationState);
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.isRunPressed == false && stateMachine.hasMovementInput)
            {
                ChangeChildState(factory.GetState<PlayerWalkState>());
                return;
            }

            if (stateMachine.hasMovementInput == false)
            {
                ChangeChildState(factory.GetState<PlayerIdleState>());
                return;
            }
        }
    }
}