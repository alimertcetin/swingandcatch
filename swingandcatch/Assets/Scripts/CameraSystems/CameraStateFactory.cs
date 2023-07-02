using TheGame.CameraSystems.States;
using TheGame.FSM;

namespace TheGame.CameraSystems
{
    public class CameraStateFactory : StateFactory<CameraFSM>
    {
        public CameraStateFactory(CameraFSM stateMachine) : base(stateMachine)
        {
            AddState(new CameraFollowFixedUpdateState(stateMachine, this));
            AddState(new CameraFollowLateUpdateState(stateMachine, this));
            AddState(new CameraShakeAndFollowLateUpdateState(stateMachine, this));
        }
    }
}