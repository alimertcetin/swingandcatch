using TheGame.FSM;
using UnityEngine;
using XIV.Core.Extensions;
using XIV.Core.Utils;

namespace TheGame.PlayerSystems.States
{
    public class PlayerJumpState : State<PlayerFSM, PlayerStateFactory>
    {
        Timer nearRopeTimer = new Timer(0.25f);
        Timer waitGroundedTimer = new Timer(0.25f);
        float yVelocity;
        
        public PlayerJumpState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            yVelocity = CalculateJumpVelocity(stateMachine.jumpHeight);
        }

        protected override void OnStateUpdate()
        {
            yVelocity += Physics.gravity.y * (stateMachine.jumpGravityScale * Time.fixedDeltaTime);
            var transform = stateMachine.transform;
            var pos = transform.position;
            pos.y += yVelocity * Time.fixedDeltaTime;
            transform.position = pos;
        }

        protected override void OnStateExit()
        {
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
            
            if (yVelocity < 0f)
            {
                ChangeRootState(factory.GetState<PlayerFallingState>());
                return;
            }
        }

        protected override void InitializeChildStates()
        {
            AddChildState(factory.GetState<OnAirMovementState>());
        }

        float CalculateJumpVelocity(float jumpHeight)
        {
            float gravity = Physics.gravity.y * stateMachine.jumpGravityScale;
            float initialVelocity = Mathf.Sqrt(2f * jumpHeight * -gravity);
            return initialVelocity;
        }
        
    }
}