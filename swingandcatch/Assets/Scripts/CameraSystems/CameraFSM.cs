using TheGame.CameraSystems.States;
using TheGame.FSM;
using UnityEngine;

namespace TheGame.CameraSystems
{
    public class CameraFSM : StateMachine
    {
        public Transform target;
        [HideInInspector] public Vector3 offset;
        [HideInInspector] public Vector3 velocity;
        public StateMachine targetStateMachine { get; private set; }

        protected override void Awake()
        {
            targetStateMachine = target.GetComponent<StateMachine>();
            offset = transform.position - target.position;
            base.Awake();
        }

        protected override State GetInitialState()
        {
            return new CameraStateFactory(this).GetState<CameraFollowLateUpdateState>();
        }
    }
}