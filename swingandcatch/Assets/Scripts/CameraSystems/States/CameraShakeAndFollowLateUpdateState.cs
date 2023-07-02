using TheGame.FSM;
using UnityEngine;
using XIV.Core.Utils;

namespace TheGame.CameraSystems.States
{
    public class CameraShakeAndFollowLateUpdateState : State<CameraFSM, CameraStateFactory>
    {
        Timer timer;
        
        public CameraShakeAndFollowLateUpdateState(CameraFSM stateMachine, CameraStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateEnter(State comingFrom)
        {
            stateMachine.shakeTransitionFlag = false;
            timer = new Timer(stateMachine.shakeDuration);
        }

        protected override void OnStateLateUpdate()
        {
            timer.Update(Time.deltaTime);
            
            var transformPosition = stateMachine.transform.position;
            var shakeAmount = stateMachine.shakeAmount;
            
            var targetPosition = Vector3.SmoothDamp(transformPosition, stateMachine.target.position + stateMachine.offset, ref stateMachine.velocity, 0.25f, 25f);
            var shake = (Mathf.PerlinNoise(Time.unscaledTime, Time.unscaledTime) * shakeAmount) * Time.deltaTime;
            targetPosition += (Vector3)Random.insideUnitCircle * shake;
            stateMachine.transform.position = targetPosition;
        }

        protected override void CheckTransitions()
        {
            if (timer.IsDone)
            {
                ChangeRootState(previousState);
                return;
            }
        }
    }
}