using TheGame.CameraSystems.States;
using TheGame.FSM;
using TheGame.PlayerSystems;
using UnityEngine;

namespace TheGame.CameraSystems
{
    public class CameraFSM : StateMachine
    {
        [HideInInspector] public Vector3 offset;
        [HideInInspector] public Vector3 velocity;
        public PlayerFSM targetStateMachine { get; private set; }
        public Transform target { get; private set; }

        protected override void Awake()
        {
            targetStateMachine = FindObjectOfType<PlayerFSM>();
            target = targetStateMachine.transform;
            offset = transform.position - target.position;
            base.Awake();
        }

        protected override State GetInitialState()
        {
            return new CameraStateFactory(this).GetState<CameraFollowLateUpdateState>();
        }
    }
}