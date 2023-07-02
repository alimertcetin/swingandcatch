using System;
using TheGame.CameraSystems.States;
using TheGame.FSM;
using TheGame.PlayerSystems;
using TheGame.ScriptableObjects.Channels;
using UnityEngine;
using XIV.Core;
using Random = UnityEngine.Random;

namespace TheGame.CameraSystems
{
    public class CameraFSM : StateMachine
    {
        public float shakeDuration;
        
        [SerializeField] FloatChannelSO cameraShakeChannel;
        
        [HideInInspector] public bool shakeTransitionFlag;
        [HideInInspector] public Vector3 offset;
        [HideInInspector] public Vector3 velocity;

        
        public float shakeAmount { get; private set; }
        public PlayerFSM targetStateMachine { get; private set; }
        public Transform target { get; private set; }

        protected override void Awake()
        {
            targetStateMachine = FindObjectOfType<PlayerFSM>();
            target = targetStateMachine.transform;
            offset = transform.position - target.position;
            base.Awake();
        }

        void OnEnable() => cameraShakeChannel.Register(OnShakeCamera);
        void OnDisable() => cameraShakeChannel.Unregister(OnShakeCamera);

        void OnShakeCamera(float shakeAmount)
        {
            shakeTransitionFlag = true;
            this.shakeAmount = shakeAmount;
        }

        protected override State GetInitialState()
        {
            return new CameraStateFactory(this).GetState<CameraFollowLateUpdateState>();
        }

        [Button]
        void Shake()
        {
            OnShakeCamera(10f);
        }
    }
}