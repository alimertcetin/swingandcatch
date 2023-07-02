using TheGame.FSM;
using TheGame.PlayerSystems.States;
using UnityEngine;

namespace TheGame.CameraSystems.States
{
    public class CameraFollowLateUpdateState : State<CameraFSM, CameraStateFactory>
    {
        public CameraFollowLateUpdateState(CameraFSM stateMachine, CameraStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateLateUpdate()
        {
            var transformPosition = stateMachine.transform.position;
            transformPosition = Vector3.SmoothDamp(transformPosition, stateMachine.target.position + stateMachine.offset, ref stateMachine.velocity, 0.25f, 25f);
            stateMachine.transform.position = transformPosition;
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.targetStateMachine.IsInState<PlayerClimbState>())
            {
                ChangeRootState(factory.GetState<CameraFollowFixedUpdateState>());
                return;
            }

            if (stateMachine.shakeTransitionFlag)
            {
                ChangeRootState(factory.GetState<CameraShakeAndFollowLateUpdateState>());
                return;
            }
        }
    }
}