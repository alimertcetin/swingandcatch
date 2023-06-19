using System;
using TheGame.FSM;
using TheGame.PlayerSystems.States;
using TheGame.ScriptableObjects.Channels;
using TheGame.VerletRope;
using UnityEngine;
using XIV.Core;
using XIV.Core.Extensions;

namespace TheGame.PlayerSystems
{
    public class PlayerFSM : StateMachine
    {
        [Header("Health and Damage Settings")]
        public float health = 100f;
        public float damageImmuneDuration = 5f;
        public float recieveDamageRadius = 1.5f;

        [Header("Movement")]
        public PlayerWalkStateDataSO walkStateDataSO;
        public PlayerRunStateDataSO runStateDataSO;
        public PlayerAirMovementStateDataSO airMovementStateDataSO;

        [Header("Jump Movement")]
        public PlayerJumpStateDataSO jumpStateDataSO;
        public PlayerGroundedStateDataSO groundedStateDataSO;
        public PlayerFallStateDataSO fallStateDataSO;

        [Header("Climb Movement")]
        public PlayerClimbStateDataSO climbStateDataSO;

        [Header("Left to Right order")]
        public Transform[] playerFeet;
        public TransformChannelSO playerDiedChannelSO;
        public TransformChannelSO playerUpdateHealthChannelSO;
        public TransformChannelSO playerReachedEndChannelSO;

        // TODO : Fix naming. Like hasMovementInput -> hasMovementInputX
        public bool hasMovementInput => Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0f;
        public Vector3 movementInput => new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
        public bool isRunPressed => Input.GetKey(KeyCode.LeftShift);
        public bool isJumpPressed => Input.GetKey(KeyCode.Space);
        
        public Rigidbody2D rb { get; private set; }
        public Vector3 velocity { get; private set; }
        public Vector3 previousPosition { get; private set; }
        
        Collider2D[] colliderBuffer = new Collider2D[2];

        protected override void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            previousPosition = transform.position;
            base.Awake();
        }

        protected override void Update()
        {
            var position = transform.position;
            velocity = position - previousPosition;
            previousPosition = position;
            base.Update();
        }

        protected override State GetInitialState()
        {
            return new PlayerStateFactory(this).GetState<PlayerGroundedState>();
        }

        public bool IsGrounded()
        {
            int layerMask = 1 << PhysicsConstants.GroundLayer;
            return CheckIsTouching(layerMask);
        }

        public bool CheckIsTouching(int layerMask)
        {
            const int DETAIL = 10;
            const float ERROR = 0.1f;

            var transform = this.transform;
            var down = -transform.up;
            var currentPosition = transform.position;
            var localScaleYHalf = transform.localScale.y * 0.5f - ERROR;
            var castStartPosition = previousPosition + down * localScaleYHalf;

            for (int i = 1; i <= DETAIL; i++)
            {
#if UNITY_EDITOR
                XIV.Core.XIVDebug.DrawLine(castStartPosition, castStartPosition + down * groundedStateDataSO.groundCheckDistance, Color.Lerp(Color.yellow, Color.green, i / (float)DETAIL));
#endif
                if (Physics.Raycast(castStartPosition, down, groundedStateDataSO.groundCheckDistance, layerMask))
                {
                    return true;
                }

                var time = i / (float)DETAIL;
                castStartPosition = Vector3.Lerp(previousPosition, currentPosition, time) + down * localScaleYHalf;
            }

            return false;
        }

        public bool GetNearestRope(out Rope rope)
        {
            rope = default;
            var position = transform.position;
            int count = Physics2D.OverlapCircleNonAlloc(position, climbStateDataSO.climbCheckRadius, colliderBuffer, 1 << PhysicsConstants.RopeLayer);
            XIVDebug.DrawCircle(position, climbStateDataSO.climbCheckRadius, Color.blue, 0.5f);
            if (count == 0) return false;
            rope = colliderBuffer.GetClosest(position, count).GetComponent<Rope>();
            return true;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (Application.isPlaying == false) return;
            Vector3 lineStart = transform.position + Vector3.right + Vector3.up * 2f;

            var healthNormalized = health / 100f;
            XIVDebug.DrawLine(lineStart, lineStart + (Vector3.right * healthNormalized), Color.Lerp(Color.red, Color.green, healthNormalized));
        }
#endif
    }
}