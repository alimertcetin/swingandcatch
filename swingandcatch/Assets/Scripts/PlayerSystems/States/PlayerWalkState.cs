using TheGame.FSM;
using TheGame.PlayerSystems.States.AnimationStates;
using UnityEngine;

namespace TheGame.PlayerSystems.States
{
    public class PlayerWalkState : State<PlayerFSM, PlayerStateFactory>
    {
        const float FEET_ANIMATION_DURATION = 0.15f;
        
        public PlayerWalkState(PlayerFSM stateMachine, PlayerStateFactory factory) : base(stateMachine, factory)
        {
        }

        protected override void OnStateUpdate()
        {
            var pos = stateMachine.transform.position;
            pos += stateMachine.horizontalMovementInput.normalized * (stateMachine.stateDatas.walkStateDataSO.walkSpeed * Time.deltaTime);
            stateMachine.Move(pos);
        }

        protected override void InitializeChildStates()
        {
            var feetAnimationState = factory.GetState<PlayerFeetMovementAnimationState>();
            feetAnimationState.animationTime = FEET_ANIMATION_DURATION;
            AddChildState(feetAnimationState);
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.isRunPressed && stateMachine.hasHorizontalMovementInput)
            {
                ChangeChildState(factory.GetState<PlayerRunSate>());
                return;
            }
            
            if (stateMachine.hasHorizontalMovementInput == false)
            {
                ChangeChildState(factory.GetState<PlayerIdleState>());
                return;
            }
        }
    }
}