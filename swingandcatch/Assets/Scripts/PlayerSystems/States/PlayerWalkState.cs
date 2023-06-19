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
            pos += stateMachine.movementInput.normalized * (stateMachine.walkStateDataSO.walkSpeed * Time.deltaTime);
            if (stateMachine.CanMove(pos, 1 << PhysicsConstants.GroundLayer))
            {
                stateMachine.transform.position = pos;
            }
        }

        protected override void InitializeChildStates()
        {
            var feetAnimationState = factory.GetState<PlayerFeetMovementAnimationState>();
            feetAnimationState.animationTime = FEET_ANIMATION_DURATION;
            AddChildState(feetAnimationState);
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.isRunPressed && stateMachine.hasMovementInput)
            {
                ChangeChildState(factory.GetState<PlayerRunSate>());
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