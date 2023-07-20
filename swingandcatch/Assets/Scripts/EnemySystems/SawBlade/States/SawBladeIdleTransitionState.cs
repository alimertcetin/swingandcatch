using System.Buffers;
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
            var newPos = Vector3.MoveTowards(pos, closestPos, stateMachine.transitionToIdleStateDataSO.goToStartPositionSpeed * Time.deltaTime);
            stateMachine.transform.position = newPos;
        }

        protected override void CheckTransitions()
        {
            if (CheckIdleTransition(out var movementStart, out var movementEnd))
            {
                var sawBladeIdleState = factory.GetState<SawBladeIdleState>();
                sawBladeIdleState.movementStart = movementStart;
                sawBladeIdleState.movementEnd = movementEnd;
                ChangeRootState(sawBladeIdleState);
                return;
            }

            if (CheckAttackTransition(out var target, out var connectedPoint))
            {
                var attackState = factory.GetState<SawBladeAttackState>();
                attackState.target = target;
                attackState.connectedPoint = connectedPoint;
                ChangeRootState(attackState);
                return;
            }
        }

        bool CheckAttackTransition(out Transform target, out Vector3 connectedPoint)
        {
            target = default;
            connectedPoint = default;
            
            var idleStart = stateMachine.idleStartPosition;
            var idleEnd = stateMachine.idleEndPosition;
            var hasMovement = Vector3.Distance(idleStart, idleEnd) > 0.0001f;
            var center = hasMovement ? Vector3.Lerp(stateMachine.idleStartPosition, stateMachine.idleEndPosition, 0.5f) : stateMachine.transform.position;
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int count = Physics2D.OverlapCircleNonAlloc(center, stateMachine.attackStateDataSO.attackFieldRadius, buffer, 1 << PhysicsConstants.PlayerLayer);

            if (count == 0)
            {
                ArrayPool<Collider2D>.Shared.Return(buffer);
                return false;
            }

            var rayBuffer = ArrayPool<RaycastHit2D>.Shared.Rent(2);
            int rayHitCount = Physics2D.LinecastNonAlloc(stateMachine.transform.position, buffer[0].transform.position, rayBuffer, 1 << PhysicsConstants.GroundLayer);
            if (rayHitCount == 0)
            {
                target = buffer[0].transform;
                connectedPoint = center;
            }

            ArrayPool<Collider2D>.Shared.Return(buffer);
            ArrayPool<RaycastHit2D>.Shared.Return(rayBuffer);
            return true;

        }

        bool CheckIdleTransition(out Vector3 movementStart, out Vector3 movementEnd)
        {
            movementStart = default;
            movementEnd = default;
            
            var pos = stateMachine.transform.position;
            var idleStart = stateMachine.idleStartPosition;
            var idleEnd = stateMachine.idleEndPosition;

            var distance1 = Vector3.Distance(pos, idleStart);
            var distance2 = Vector3.Distance(pos, idleEnd);

            var closestPos = distance2 < distance1 ? idleEnd : idleStart;
            if (Vector3.Distance(pos, closestPos) < 0.0001f)
            {
                movementStart = distance2 < distance1 ? idleEnd : idleStart;
                movementEnd = distance2 < distance1 ? idleStart : idleEnd;
                return true;
            }

            return false;
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