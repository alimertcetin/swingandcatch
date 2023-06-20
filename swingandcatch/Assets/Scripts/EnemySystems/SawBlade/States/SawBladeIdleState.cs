using System.Buffers;
using TheGame.FSM;
using UnityEngine;
using XIV.Core.Utils;

namespace TheGame.EnemySystems.SawBlade.States
{
    public class SawBladeIdleState : State<SawBladeFSM, SawBladeStateFactory>
    {
        public Vector3 movementStart;
        public Vector3 movementEnd;
        Timer timer;
        EasingFunction.Function easingFunc;
        Collider2D[] buffer;
        bool hasMovement;

        public SawBladeIdleState(SawBladeFSM stateMachine, SawBladeStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            var idleStateData = stateMachine.idleStateDataSO;
            var distance = Vector3.Distance(movementStart, movementEnd);
            if (distance < 0.0001f)
            {
                hasMovement = false;
                buffer = ArrayPool<Collider2D>.Shared.Rent(2);
                return;
            }

            hasMovement = true;
            
            var time = distance / idleStateData.idleMovementSpeed;
            timer = new Timer(time);
            easingFunc = EasingFunction.GetEasingFunction(idleStateData.ease);
            buffer = ArrayPool<Collider2D>.Shared.Rent(2);
        }

        protected override void OnStateUpdate()
        {
            if (hasMovement == false) return;
            timer.Update(Time.deltaTime);
            stateMachine.transform.localPosition = Vector3.Lerp(movementStart, movementEnd, easingFunc.Invoke(0f, 1f, timer.NormalizedTime));
            if (timer.IsDone)
            {
                timer.Restart();
                (movementStart, movementEnd) = (movementEnd, movementStart);
            }
        }

        protected override void OnStateExit()
        {
            ArrayPool<Collider2D>.Shared.Return(buffer);
        }

        protected override void CheckTransitions()
        {
            var center = hasMovement ? Vector3.Lerp(stateMachine.idleStartPosition, stateMachine.idleEndPosition, 0.5f) : stateMachine.transform.position;
            int count = Physics2D.OverlapCircleNonAlloc(center, stateMachine.attackStateDataSO.attackFieldRadius, buffer, 1 << PhysicsConstants.PlayerLayer);

            if (count > 0)
            {
                var attackState = factory.GetState<SawBladeAttackState>();
                attackState.target = buffer[0].transform;
                attackState.connectedPoint = center;
                ChangeRootState(attackState);
                return;
            }
        }
    }
}