using TheGame.FSM;
using TheGame.PlayerSystems.States.AnimationStates;
using UnityEngine;

namespace TheGame.PlayerSystems.States
{
    public class PlayerRunSate : State<PlayerFSM, PlayerStateFactory>
    {
        const float FEET_ANIMATION_DURATION = 0.1f;
        PlayerGroundedState groundedState;
        
        public PlayerRunSate(PlayerFSM stateMachine, PlayerStateFactory factory) : base(stateMachine, factory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            groundedState = factory.GetState<PlayerGroundedState>();
        }

        protected override void OnStateUpdate()
        {
            var pos = stateMachine.transform.position;
            pos += Vector3.right * (groundedState.horizontalMovementInput * (stateMachine.stateDatas.runStateDataSO.runSpeed * Time.deltaTime));
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
            if (groundedState.horizontalMovementPressed == false)
            {
                ChangeChildState(factory.GetState<PlayerIdleState>());
                return;
            }
            
            if (groundedState.runPressed == false)
            {
                ChangeChildState(factory.GetState<PlayerWalkState>());
                return;
            }
        }
    }
}