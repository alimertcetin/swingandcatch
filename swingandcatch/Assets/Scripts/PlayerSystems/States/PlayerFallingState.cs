using TheGame.FSM;
using UnityEngine;

namespace TheGame.PlayerSystems.States
{
    public class PlayerFallingState : State<PlayerFSM, PlayerStateFactory>
    {
        const float MAX_DISTANCE_TO_ROPE_SEGMENT = 0.5f;
        float yVelocity;
        float fallingTime;
        
        public PlayerFallingState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            yVelocity = stateMachine.velocity.y;
            fallingTime = 0f;
        }

        protected override void OnStateUpdate()
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            yVelocity += Physics.gravity.y * (stateMachine.fallGravityScale * fixedDeltaTime + Mathf.Clamp(fallingTime, 0f, 10f) * fixedDeltaTime);
            fallingTime += Time.deltaTime;
            yVelocity = Mathf.Clamp(yVelocity, -18f, 0f);
            
            var transform = stateMachine.transform;
            var pos = transform.position;
            pos.y += yVelocity * fixedDeltaTime;
            transform.position = pos;
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.IsGrounded())
            {
                ChangeRootState(factory.GetState<PlayerGroundedState>());
                return;
            }
            
            if (stateMachine.GetNearestRope(out var rope))
            {
                var pos = stateMachine.transform.position;
                ref var closestRopePoint = ref rope.GetClosestPoint(pos);
                if (closestRopePoint.index != 0 && Vector3.Distance(closestRopePoint.position, pos) < MAX_DISTANCE_TO_ROPE_SEGMENT)
                {
                    ChangeRootState(factory.GetState<PlayerClimbState>());
                    return;
                }
            }
        }

        protected override void InitializeChildStates()
        {
            AddChildState(factory.GetState<OnAirMovementState>());
        }
    }
}