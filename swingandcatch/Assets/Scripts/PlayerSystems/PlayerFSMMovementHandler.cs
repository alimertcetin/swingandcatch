using System;
using System.Buffers;
using TheGame.Interfaces;
using UnityEngine;
using XIV.Core;
using XIV.Core.XIVMath;

namespace TheGame.PlayerSystems
{
    [RequireComponent(typeof(PlayerFSM))]
    public class PlayerFSMMovementHandler : MonoBehaviour, IMovementHandler
    {
        // var collisionMask = 1 << PhysicsConstants.GroundLayer;
        [SerializeField] LayerMask movementCollisionMask;

        PlayerFSM playerFSM;
        Collider2D collider2D;

        public Vector3 velocity;
        public Vector3 previousPosition;

        void Awake()
        {
            playerFSM = GetComponent<PlayerFSM>();
            collider2D = GetComponent<Collider2D>();
            previousPosition = transform.position;
        }

        void Update()
        {
            var position = transform.position;
            velocity = position - previousPosition;
            previousPosition = position;
        }
        
        void IMovementHandler.SyncPosition() => previousPosition = transform.position;
        Vector3 IMovementHandler.GetPreviousPosition() => previousPosition;

        Vector3 IMovementHandler.GetCurrentVelocity() => velocity;

        public bool Move(Vector3 targetPosition)
        {
            var distance = playerFSM.stateDatas.groundedStateDataSO.groundCheckDistance;
            
            var canMove = CanMove(targetPosition, collider2D.bounds, distance, movementCollisionMask, out var positionBeforeCollision);
            var nextPos = canMove ? targetPosition : positionBeforeCollision;
            transform.position = nextPos;
            
            return canMove;
        }

        bool IMovementHandler.CanMove(Vector3 targetPosition)
        {
            var distance = playerFSM.stateDatas.groundedStateDataSO.groundCheckDistance;
            return CanMove(targetPosition, collider2D.bounds, distance, movementCollisionMask, out _);
        }

        bool IMovementHandler.CheckIsTouching(int layerMask)
        {
            return CheckIsTouching(playerFSM, layerMask);
        }

        int IMovementHandler.CheckIsTouchingNonAlloc(Collider2D[] buffer, int layerMask)
        {
            return CheckIsTouchingNonAlloc(playerFSM, buffer, layerMask);
        }

        bool CanMove(Vector3 targetPosition, Bounds bounds, float maxDistance, int layerMask, out Vector3 positionBeforeCollision)
        {
            const int DETAIL = 20;
            
            var transformPosition = playerFSM.transform.position;
            
            var dir = (targetPosition - transformPosition).normalized;
            var collisionTestPoints = ArrayPool<Vector3>.Shared.Rent(DETAIL);
            GetCollisionTestPoints(DETAIL, transformPosition, dir, bounds, collisionTestPoints);

            var testPosition = previousPosition;
            positionBeforeCollision = testPosition;
            bool hasCollision = false;

            while (hasCollision == false)
            {
                hasCollision = HasCollision(testPosition, collisionTestPoints, DETAIL, dir, maxDistance, layerMask);
                positionBeforeCollision = testPosition;
                // hasCollision might not be true ever, we actually rely on below line to break the loop
                if ((targetPosition - testPosition).sqrMagnitude - Mathf.Epsilon < Mathf.Epsilon) break;
                testPosition = Vector3.MoveTowards(testPosition, targetPosition, 0.01f);
            }

            ArrayPool<Vector3>.Shared.Return(collisionTestPoints);
            return hasCollision == false;
        }

        static void GetCollisionTestPoints(int detail, Vector3 position, Vector3 dir, Bounds bounds, Vector3[] buffer)
        {
            const float ERROR = 0.01f;
            const float HALF_ANGLE = 15f;
            for (int i = 0; i < detail; i++)
            {
                var t = i / (float)(detail - 1);
                var rotT = XIVMathf.Remap(t, 0f, 1f, -1f, 1f);
                var rot = Quaternion.AngleAxis(rotT * HALF_ANGLE, Vector3.forward);
                var pos = position + rot * dir;
                var posOnCollider = bounds.ClosestPoint(pos);
                var d = (position - posOnCollider).normalized * ERROR;
                buffer[i] = posOnCollider - position + d;
            }
        }
        
        static bool HasCollision(Vector3 position, Vector3[] localTestPoints, int testPointLength, Vector3 movementDirection, float distance, int layerMask)
        {
            const float ERROR = 0.01f;
            var hitBufer = ArrayPool<RaycastHit2D>.Shared.Rent(2);
            int hitCount = 0;
            for (int i = 0; i < testPointLength && hitCount == 0; i++)
            {
                var pStart = localTestPoints[i] + position;
                var direction = (pStart - position).normalized;
                var dot = Vector3.Dot(direction, movementDirection);
                dot *= dot;
                
                hitCount = Physics2D.RaycastNonAlloc(pStart, direction, hitBufer, distance * dot + ERROR, layerMask);
                var color = hitCount == 0 ? Color.Lerp(Color.green, Color.white, i / (float)(testPointLength - 1)) : Color.red;
#if UNITY_EDITOR
                XIVDebug.DrawLine(pStart, pStart + direction * (distance * dot + ERROR), color);
#endif
            }
            ArrayPool<RaycastHit2D>.Shared.Return(hitBufer);
            return hitCount > 0;
        }
        
        static bool CheckIsTouching(PlayerFSM playerFSM, int layerMask)
        {
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int hitCount = CheckIsTouchingNonAlloc(playerFSM, buffer, layerMask);
            ArrayPool<Collider2D>.Shared.Return(buffer);
            return hitCount > 0;
        }
        
        static int CheckIsTouchingNonAlloc(PlayerFSM playerFSM, Collider2D[] buffer, int layerMask)
        {
            var position = playerFSM.movementHandler.GetPreviousPosition();
            var targetPosition = playerFSM.transform.position;
            var capsuleSize = playerFSM.bottomColliderSize;
            int hitCount = 0;

            do
            {
                var capsuleCenter = position + playerFSM.bottomColliderPosLocal;
                hitCount = Physics2D.OverlapCapsuleNonAlloc(capsuleCenter, capsuleSize, CapsuleDirection2D.Horizontal, 0f, buffer, layerMask);
#if UNITY_EDITOR
                var halfExtends = capsuleSize * 0.5f;
                var radius = capsuleSize.y * 0.5f;
                XIVDebug.DrawRectangle(capsuleCenter, halfExtends, Quaternion.LookRotation(Vector3.forward));
                XIVDebug.DrawCircle(capsuleCenter + Vector3.left * (halfExtends.x - radius), radius);
                XIVDebug.DrawCircle(capsuleCenter + Vector3.right * (halfExtends.x - radius), radius);
#endif
                position = Vector3.MoveTowards(position, targetPosition, 0.01f);
                
            } while ((targetPosition - position).sqrMagnitude - Mathf.Epsilon > Mathf.Epsilon && hitCount == 0);

            return hitCount;
        }
    }
}