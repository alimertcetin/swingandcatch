using System;
using System.Buffers;
using TheGame.FSM;
using TheGame.HazzardSystems;
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
        public TransformChannelSO playerReachedEndChannelSO;
        public FloatChannelSO updatePlayerHealthChannel;

        // TODO : Fix naming. Like hasMovementInput -> hasMovementInputX
        public bool hasMovementInput => Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0f;
        public Vector3 movementInput => new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
        public bool isRunPressed => Input.GetKey(KeyCode.LeftShift);
        public bool isJumpPressed => Input.GetKey(KeyCode.Space);

        public Vector3 velocity { get; private set; }
        public Vector3 previousPosition { get; private set; }

        Collider2D[] colliderBuffer = new Collider2D[2];
        [HideInInspector] public bool damageImmune;

        protected override void Awake()
        {
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
            const float BOUND_Y_HALF = 0.2f;
            
            var t = transform;
            var halfExtends = t.localScale * 0.5f;
            var bottomMiddle = t.position + -t.up * halfExtends.y;

            var size = new Vector3(halfExtends.x * 1.5f, BOUND_Y_HALF, halfExtends.z);
            
            var buffer = ArrayPool<Collider>.Shared.Rent(2);
            int hitCount = Physics.OverlapBoxNonAlloc(bottomMiddle, size * 0.5f, buffer, t.rotation, layerMask);
            XIVDebug.DrawBounds(new Bounds(bottomMiddle, size));
            ArrayPool<Collider>.Shared.Return(buffer);
            return hitCount > 0;
        }

        public bool CanMove(Vector3 newPosition, int layerMask, bool setLastPossiblePosition)
        {
            const int DETAIL = 10;
            const float ERROR = 0.1f;

            var transform = this.transform;
            var dir = (newPosition - previousPosition).normalized;
            var localScaleYHalf = transform.localScale.y * 0.5f - ERROR;
            var position = previousPosition;
            var positionBefore = position;
            var castStartPosition = position + dir * localScaleYHalf;

            for (int i = 0; i <= DETAIL; i++)
            {
#if UNITY_EDITOR
                XIV.Core.XIVDebug.DrawLine(castStartPosition, castStartPosition + dir * groundedStateDataSO.groundCheckDistance, Color.Lerp(Color.blue, Color.red, i / (float)DETAIL));
#endif
                if (Physics.Raycast(castStartPosition, dir, groundedStateDataSO.groundCheckDistance, layerMask))
                {
                    if (setLastPossiblePosition)
                    {
                        transform.position = positionBefore;
                    }
                    return false;
                }

                var time = i / (float)DETAIL;
                positionBefore = position;
                position = Vector3.Lerp(previousPosition, newPosition, time);
                castStartPosition = position + dir * localScaleYHalf;
            }

            return true;
        }

        public bool GetNearestRope(out Rope rope)
        {
            rope = default;
            var position = transform.position;
            int count = Physics2D.OverlapCircleNonAlloc(position, climbStateDataSO.climbCheckRadius, colliderBuffer, 1 << PhysicsConstants.RopeLayer);
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
        public void OnHazzardHit(Collider2D coll)
        {
            var hazzardMono = coll.transform.GetComponent<HazzardMono>();
            var damageAmount = hazzardMono.damageAmount;
            health -= damageAmount;
            updatePlayerHealthChannel.RaiseEvent(health / 100f);
            hazzardMono.RaiseEvent(transform);
        }
    }
}