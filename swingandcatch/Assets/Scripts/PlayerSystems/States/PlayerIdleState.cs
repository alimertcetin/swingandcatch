using TheGame.FSM;
using TheGame.PlayerSystems.States.AnimationStates;
using TheGame.Scripts.InputSystems;

namespace TheGame.PlayerSystems.States
{
    public class PlayerIdleState : State<PlayerFSM, PlayerStateFactory>
    {
        const float BREATH_ANIMATION_DURATION = 2.5f;
        
        public PlayerIdleState(PlayerFSM stateMachine, PlayerStateFactory factory) : base(stateMachine, factory)
        {
        }
        
        protected override void InitializeChildStates()
        {
            var breathAnimationState = factory.GetState<PlayerBreathAnimationState>();
            breathAnimationState.animationDuration = BREATH_ANIMATION_DURATION;
            AddChildState(breathAnimationState);
        }

        protected override void CheckTransitions()
        {
            if (InputManager.Inputs.PlayerGrounded.HorizontalMovement.IsPressed())
            {
                ChangeChildState(factory.GetState<PlayerWalkState>());
                return;
            }
        }

    }
}