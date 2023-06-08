using TheGame.FSM;
using UnityEngine;
using XIV.Core.Extensions;
using XIV.Core.Utils;

namespace TheGame.PlayerSystems.States
{
    public class PlayerJumpState : State<PlayerFSM, PlayerStateFactory>
    {
        float initialYPos;
        Timer jumpTimer;
        Timer nearRopeTimer = new Timer(0.25f);
        Timer waitGroundedTimer = new Timer(0.25f);
        
        public PlayerJumpState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            initialYPos = stateMachine.transform.position.y;
            jumpTimer = new Timer(Mathf.Sqrt(stateMachine.jumpForce / (0.5f * stateMachine.jumpHeight)));
        }

        protected override void OnStateUpdate()
        {
            var transform = stateMachine.transform;
            var transformPosition = transform.position;
            var normalizedTime = EasingFunction.SmoothStop2(jumpTimer.NormalizedTime);
            transformPosition = transformPosition.SetY(initialYPos + (stateMachine.jumpHeight * normalizedTime - 0.5f * Physics2D.gravity.magnitude * normalizedTime * normalizedTime));
            transform.position = transformPosition;
            jumpTimer.Update(Time.deltaTime);
        }

        protected override void OnStateExit()
        {
            jumpTimer.Restart();
            nearRopeTimer.Restart();
            waitGroundedTimer.Restart();
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.GetNearestRope(out var rope) && nearRopeTimer.Update(Time.deltaTime) && rope.GetClosestPoint(stateMachine.transform.position).index != 0)
            {
                ChangeRootState(factory.GetState<PlayerClimbState>());
                return;
            }
            
            if (stateMachine.IsGrounded() && waitGroundedTimer.Update(Time.deltaTime))
            {
                ChangeRootState(factory.GetState<PlayerGroundedState>());
                return;
            }
            
            if (jumpTimer.IsDone)
            {
                ChangeRootState(factory.GetState<PlayerFallingState>());
                return;
            }
        }

        protected override void InitializeChildStates()
        {
            AddChildState(factory.GetState<OnAirMovementState>());
        }
        
        
    }
}