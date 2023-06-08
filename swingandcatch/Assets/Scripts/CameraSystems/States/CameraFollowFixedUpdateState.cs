using TheGame.FSM;
using TheGame.PlayerSystems.States;
using UnityEngine;

namespace TheGame.CameraSystems.States
{
    public class CameraFollowFixedUpdateState : State<CameraFSM, CameraStateFactory>
    {
        public CameraFollowFixedUpdateState(CameraFSM stateMachine, CameraStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
        }

        protected override void OnStateFixedUpdate()
        {
            var transformPosition = stateMachine.transform.position;
            transformPosition = Vector3.SmoothDamp(transformPosition, stateMachine.target.position + stateMachine.offset, ref stateMachine.velocity, 0.25f, 25f);
            stateMachine.transform.position = transformPosition;
        }

        protected override void CheckTransitions()
        {
            if (stateMachine.targetStateMachine.IsInState<PlayerClimbState>() == false)
            {
                ChangeRootState(factory.GetState<CameraFollowLateUpdateState>());
                return;
            }
        }
    }
}