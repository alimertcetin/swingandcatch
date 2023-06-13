using System.Collections.Generic;
using TheGame.FSM;
using TheGame.VerletRope;
using UnityEngine;
using UnityEngine.Pool;
using XIV.Core;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.TweenSystem;

namespace TheGame.PlayerSystems.States
{
    public class PlayerClimbState : State<PlayerFSM, PlayerStateFactory>
    {
        Rope currentRope;
        float currentT;
        List<Vector3> positionBuffer;
        Vector3 movementInput;

        public PlayerClimbState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            stateMachine.GetNearestRope(out currentRope);
            positionBuffer = ListPool<Vector3>.Get();
            
            Vector3 pos = stateMachine.transform.position;

            currentRope.GetPositionsNonAlloc(positionBuffer);
            currentT = Mathf.Clamp01(SplineMath.GetTime(pos, positionBuffer));
            
            Vector3 positionOnSegment = SplineMath.GetPoint(positionBuffer, currentT);
            stateMachine.transform.position = positionOnSegment;
            
            currentRope.AddForce(positionOnSegment, stateMachine.velocity.normalized * stateMachine.ropeSwingInitialForce);
            
            stateMachine.CancelTween();
            stateMachine.XIVTween()
                .Scale(stateMachine.transform.localScale, Vector3.one * 1.25f, 0.15f, EasingFunction.EaseInOutBounce, true)
                .Start();
        }

        protected override void OnStateUpdate()
        {
            movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
        }

        protected override void OnStateFixedUpdate()
        {
            var hasMovementInput = movementInput.sqrMagnitude > Mathf.Epsilon;
            positionBuffer.Clear();
            currentRope.GetPositionsNonAlloc(positionBuffer);

            if (hasMovementInput == false)
            {
                var currentPosition = SplineMath.GetPoint(positionBuffer, currentT);
                stateMachine.transform.position = currentPosition;
                
                XIVDebug.DrawCircle(currentPosition, 0.25f, Color.yellow, 5f);
                return;
            }

            currentT -= stateMachine.climbSpeed * Time.deltaTime * movementInput.y;
            currentT = Mathf.Clamp01(currentT);
            var nextPosition = SplineMath.GetPoint(positionBuffer, currentT);
            stateMachine.transform.position = nextPosition;

            if (Mathf.Abs(movementInput.x) > 0f) currentRope.AddForce(nextPosition, movementInput.normalized * stateMachine.ropeSwingForce);
        }

        protected override void OnStateExit()
        {
            ListPool<Vector3>.Release(positionBuffer);
            stateMachine.CancelTween();
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.isJumpPressed)
            {
                ChangeRootState(factory.GetState<PlayerJumpState>());
                return;
            }
        }
    }
}