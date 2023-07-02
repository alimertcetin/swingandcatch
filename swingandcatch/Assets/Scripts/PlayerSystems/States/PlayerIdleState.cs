using TheGame.FSM;
using TheGame.PlayerSystems.States.AnimationStates;

namespace TheGame.PlayerSystems.States
{
    public class PlayerIdleState : State<PlayerFSM, PlayerStateFactory>
    {
        const float BREATH_ANIMATION_DURATION = 2.5f;
        PlayerGroundedState groundedState;
        
        public PlayerIdleState(PlayerFSM stateMachine, PlayerStateFactory factory) : base(stateMachine, factory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            groundedState = factory.GetState<PlayerGroundedState>();
        }

        protected override void InitializeChildStates()
        {
            var breathAnimationState = factory.GetState<PlayerBreathAnimationState>();
            breathAnimationState.animationDuration = BREATH_ANIMATION_DURATION;
            AddChildState(breathAnimationState);
        }

        protected override void CheckTransitions()
        {
            if (groundedState.runPressed && groundedState.horizontalMovementPressed)
            {
                ChangeChildState(factory.GetState<PlayerRunSate>());
                return;
            }
            
            if (groundedState.horizontalMovementPressed)
            {
                ChangeChildState(factory.GetState<PlayerWalkState>());
                return;
            }
        }
    }
}