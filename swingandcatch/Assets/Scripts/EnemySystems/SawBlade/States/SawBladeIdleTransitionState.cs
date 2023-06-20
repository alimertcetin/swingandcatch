using TheGame.FSM;
using UnityEngine;

namespace TheGame.EnemySystems.SawBlade.States
{
    public class SawBladeIdleTransitionState : State<SawBladeFSM, SawBladeStateFactory>
    {
        public SawBladeIdleTransitionState(SawBladeFSM stateMachine, SawBladeStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateUpdate()
        {
            var pos = stateMachine.transform.position;
            var closestPos = GetClosestIdlePosition(pos);
            var newPos = Vector3.MoveTowards(pos, closestPos, stateMachine.idleStateDataSO.idleMovementSpeed * Time.deltaTime);
            stateMachine.transform.position = newPos;
        }

        protected override void CheckTransitions()
        {
            var pos = stateMachine.transform.position;
            var idleStart = stateMachine.idleStartPosition;
            var idleEnd = stateMachine.idleEndPosition;

            var distance1 = Vector3.Distance(pos, idleStart);
            var distance2 = Vector3.Distance(pos, idleEnd);

            var closestPos = distance2 < distance1 ? idleEnd : idleStart;
            if (Vector3.Distance(pos, closestPos) < 0.0001f)
            {
                var sawBladeIdleState = factory.GetState<SawBladeIdleState>();
                sawBladeIdleState.movementStart = distance2 < distance1 ? idleEnd : idleStart;
                sawBladeIdleState.movementEnd = distance2 < distance1 ? idleStart : idleEnd;
                ChangeRootState(sawBladeIdleState);
            }
        }

        Vector3 GetClosestIdlePosition(Vector3 pos)
        {
            var idleStart = stateMachine.idleStartPosition;
            var idleEnd = stateMachine.idleEndPosition;

            var distance1 = Vector3.Distance(pos, idleStart);
            var distance2 = Vector3.Distance(pos, idleEnd);

            var closestPos = distance2 < distance1 ? idleEnd : idleStart;
            return closestPos;
        }
    }
}