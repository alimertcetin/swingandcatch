using TheGame.FSM;
using UnityEngine;

namespace TheGame.PlayerSystems.States
{
    public class PlayerFallingState : State<PlayerFSM, PlayerStateFactory>
    {
        const float MAX_DISTANCE_TO_ROPE_SEGMENT = 0.5f;
        
        public PlayerFallingState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
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
            stateMachine.transform.position += Physics.gravity * (stateMachine.fallSpeed * Time.deltaTime);
        }

        protected override void InitializeChildStates()
        {
            AddChildState(factory.GetState<OnAirMovementState>());
        }
    }
}