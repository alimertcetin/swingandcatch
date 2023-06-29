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
using XIV.Core.XIVMath;

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

        [Tooltip("Left to Right order")]
        public Transform[] playerFeet;
        public Transform playerVisualTransform;

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
        Collider2D coll2D;

        protected override void Awake()
        {
            previousPosition = transform.position;
            coll2D = GetComponent<Collider2D>();
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
            var buffer = ArrayPool<Collider>.Shared.Rent(2);
            int hitCount = CheckIsTouchingNonAlloc(buffer, layerMask);
            ArrayPool<Collider>.Shared.Return(buffer);
            return hitCount > 0;
        }

        public int CheckIsTouchingNonAlloc(Collider[] buffer, int layerMask)
        {
            const float BOUND_Y_HALF = 0.07f;
            
            var t = transform;
            var localScale = t.localScale;
            var halfExtends = localScale * 0.5f;
            var bottomMiddle = t.position + -t.up * halfExtends.y;

            var size = halfExtends * 0.75f;
            size.y = BOUND_Y_HALF;
            
            int hitCount = Physics.OverlapBoxNonAlloc(bottomMiddle, size, buffer, t.rotation, layerMask);
            XIVDebug.DrawBounds(new Bounds(bottomMiddle, size * 2f));
            return hitCount;
        }

        public bool CanMove(Vector3 newPosition, int layerMask, bool setLastPossiblePosition)
        {
            const int DETAIL = 20;

            var transformPosition = transform.position;
            var dir = (newPosition - transformPosition).normalized;
            var testPointBuffer = ArrayPool<Vector3>.Shared.Rent(DETAIL);
            GetTestPointsLocal(DETAIL, transformPosition, dir, testPointBuffer);

            var maxDistance = groundedStateDataSO.groundCheckDistance;
            var testPosition = previousPosition;
            var positionBeforeCollision = testPosition;
            bool hasCollision = false;

            while (hasCollision == false)
            {
                hasCollision = HasCollision(testPosition, testPointBuffer, DETAIL, dir, maxDistance, layerMask);
                positionBeforeCollision = testPosition;
                // hasCollision might not be true ever, we actually rely on below line to break the loop
                if ((newPosition - testPosition).sqrMagnitude - Mathf.Epsilon < Mathf.Epsilon) break;
                testPosition = Vector3.MoveTowards(testPosition, newPosition, 0.01f);
            }
            
            if (hasCollision && setLastPossiblePosition) transform.position = positionBeforeCollision;

            ArrayPool<Vector3>.Shared.Return(testPointBuffer);
            return hasCollision == false;
        }

        void GetTestPointsLocal(int detail, Vector3 position, Vector3 dir, Vector3[] buffer)
        {
            const float ERROR = 0.01f;
            var bounds = coll2D.bounds;
            for (int i = 0; i < detail; i++)
            {
                var t = i / (float)(detail - 1);
                var rotT = XIVMathf.Remap(t, 0f, 1f, -1f, 1f);
                var rot = Quaternion.AngleAxis(rotT * 20f, Vector3.forward);
                var pos = position + rot * dir;
                var posOnCollider = bounds.ClosestPoint(pos);
                var d = (position - posOnCollider).normalized * ERROR;
                buffer[i] = posOnCollider - position + d;
            }
        }

        bool HasCollision(Vector3 position, Vector3[] localTestPoints, int testPointLength, Vector3 movementDirection, float distance, int layerMask)
        {
            const float ERROR = 0.01f;
            var hitBufer = ArrayPool<RaycastHit>.Shared.Rent(2);
            int hitCount = 0;
            for (int i = 0; i < testPointLength && hitCount == 0; i++)
            {
                var pStart = localTestPoints[i] + position;
                var direction = (pStart - position).normalized;
                var dot = Vector3.Dot(direction, movementDirection);
                dot *= dot;
                
                hitCount = Physics.RaycastNonAlloc(pStart, direction, hitBufer, distance * dot + ERROR, layerMask);
                var color = hitCount == 0 ? Color.Lerp(Color.green, Color.white, i / (float)(testPointLength - 1)) : Color.red;
                XIVDebug.DrawLine(pStart, pStart + direction * (distance * dot + ERROR), color);
            }
            ArrayPool<RaycastHit>.Shared.Return(hitBufer);
            return hitCount > 0;
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