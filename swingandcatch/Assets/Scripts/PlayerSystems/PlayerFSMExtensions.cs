using System.Buffers;
using TheGame.VerletRope;
using UnityEngine;
using XIV.Core;
using XIV.Core.Extensions;
using XIV.Core.XIVMath;

namespace TheGame.PlayerSystems
{
    public static class PlayerFSMExtensions
    {
        public static bool CheckIsTouching(this PlayerFSM playerFSM, int layerMask)
        {
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int hitCount = CheckIsTouchingNonAlloc(playerFSM, buffer, layerMask);
            ArrayPool<Collider2D>.Shared.Return(buffer);
            return hitCount > 0;
        }
        
        public static int CheckIsTouchingNonAlloc(this PlayerFSM playerFSM, Collider2D[] buffer, int layerMask)
        {
            var position = playerFSM.previousPosition;
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

        public static bool CanMove(this PlayerFSM playerFSM, Vector3 targetPosition, Bounds bounds, float maxDistance, int layerMask, out Vector3 positionBeforeCollision)
        {
            const int DETAIL = 20;
            
            var previousPosition = playerFSM.previousPosition;
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
        
        public static bool GetNearestRope(this PlayerFSM playerFSM, out Rope rope)
        {
            var position = playerFSM.transform.position;
            var checkRadius = playerFSM.stateDatas.climbStateDataSO.climbCheckRadius;
            
            rope = default;
            var colliderBuffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int count = Physics2D.OverlapCircleNonAlloc(position, checkRadius, colliderBuffer, 1 << PhysicsConstants.RopeLayer);
            if (count == 0)
            {
                ArrayPool<Collider2D>.Shared.Return(colliderBuffer);
                return false;
            }
            rope = colliderBuffer.GetClosest(position, count).GetComponent<Rope>();
            ArrayPool<Collider2D>.Shared.Return(colliderBuffer);
            return count > 0;
        }
    }
}