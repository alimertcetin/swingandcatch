using TheGame.FSM;
using TheGame.PlayerSystems.States.AnimationStates;
using TheGame.Scripts.InputSystems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheGame.PlayerSystems.States
{
    public class PlayerWalkState : State<PlayerFSM, PlayerStateFactory>
    {
        const float FEET_ANIMATION_DURATION = 0.15f;
        PlayerGroundedState groundedState;
        
        public PlayerWalkState(PlayerFSM stateMachine, PlayerStateFactory factory) : base(stateMachine, factory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            groundedState = factory.GetState<PlayerGroundedState>();
        }

        protected override void OnStateUpdate()
        {
            var pos = stateMachine.transform.position;
            pos += Vector3.right * (groundedState.horizontalMovementInput * (stateMachine.stateDatas.walkStateDataSO.walkSpeed * Time.deltaTime));
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
            
            if (groundedState.runPressed)
            {
                ChangeChildState(factory.GetState<PlayerRunSate>());
                return;
            }
        }
    }
}