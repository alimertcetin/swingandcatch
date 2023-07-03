using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using XIV.Core.Collections;
using XIV.Core.XIVMath;

namespace TheGame.VerletRope
{
    public class Rope : MonoBehaviour
    {
        [SerializeField] int segments = 10;
        [SerializeField] float segmentLength = 1f;
        [SerializeField] int correctionSteps = 30;
        [SerializeField] Vector3 gravity = Physics.gravity;
        [SerializeField] float drag = 0.1f;
        [SerializeField] Vector3 creationDirection = Vector3.right;

        public int Segments => segments;
        public float SegmentLength => segmentLength;
        public Vector3 Gravity => gravity;
        public float Drag => drag;
        public float FixedLength => segments * segmentLength;
        public bool isAwake;

        RopePoint[] ropePoints;
        Dictionary<int, DynamicArray<IRopeConstraint>> constraints = new Dictionary<int, DynamicArray<IRopeConstraint>>();
        Camera cam;

        void Awake()
        {
            cam = Camera.main;
            ropePoints = new RopePoint[segments];
            creationDirection.Normalize();
            Vector3 segmentOffset = creationDirection * segmentLength;
            var transformPosition = transform.position;

            for (int i = 0; i < segments; i++)
            {
                ref var p = ref ropePoints[i];
                p.index = i;
                p.position = transformPosition + i * segmentOffset;
                p.previousPosition = p.position;
                p.force = Vector3.zero;
                p.normal = Vector3.up;
            }
            isAwake = true;
            Simulate();
            ApplyConstraints();
            CalculatePointNormals();
        }
        
        void FixedUpdate()
        {
            CheckShouldSleep();
            if (isAwake == false) return;
            Simulate();
            ApplyConstraints();
            CalculatePointNormals();
        }

        void ApplyConstraints()
        {
            foreach (KeyValuePair<int, DynamicArray<IRopeConstraint>> keyValuePair in constraints)
            {
                int constraintsCount = keyValuePair.Value.Count;
                for (int i = 0; i < constraintsCount; i++)
                {
                    keyValuePair.Value[i].Apply(ropePoints);
                }
            }
        }

        void Simulate()
        {
            float dt = Time.fixedDeltaTime * Time.timeScale;
            for (int i = 0; i < segments; i++)
            {
                ref RopePoint p = ref ropePoints[i];

                Vector3 velocity = p.velocity;
                Vector3 dragForce = -drag * velocity;
                Vector3 newPosition = p.position + velocity + p.force * (dt * dt);
                Vector3 newVel = newPosition - p.position;
                p.previousPosition = p.position;
                p.position = newPosition;
                p.force = newVel + gravity + dragForce;
            }

            int mask = 1 << PhysicsConstants.GroundLayer;
            var buffer = ArrayPool<RaycastHit2D>.Shared.Rent(4);
            
            for (int i = 0; i < correctionSteps; i++)
            {
                for (int j = 0; j < segments - 1; j++)
                {
                    ref RopePoint p0 = ref ropePoints[j];
                    ref RopePoint p1 = ref ropePoints[j + 1];

                    float distance = Vector3.Distance(p0.position, p1.position);
                    float error = Mathf.Abs(distance - segmentLength);

                    Vector3 dir = (distance < segmentLength ? (p0.position - p1.position) : (p1.position - p0.position)).normalized;
                    Vector3 correction = dir * (error * 0.5f);
                    
                    p0.position += correction;
                    p1.position -= correction;
                }
                
                SolveCollisions(buffer, mask);
            }
            ArrayPool<RaycastHit2D>.Shared.Return(buffer);
        }
        
        void SolveCollisions(RaycastHit2D[] buffer, int layerMask)
        {
            const float COLLISION_FORCE = 0.05f;
            
            for (int i = 0; i < segments; i++)
            {
                ref var ropePoint = ref ropePoints[i];
                int hitCount = Physics2D.LinecastNonAlloc(ropePoint.previousPosition, ropePoint.position, buffer, layerMask);

                if (hitCount == 0) continue;

                RaycastHit2D hit = default;
                float distance = float.MaxValue;
                var pos2D = (Vector2)ropePoint.position;
                for (int j = 0; j < hitCount; j++)
                {
                    var diff = pos2D - buffer[j].point;
                    var tempDistance = diff.sqrMagnitude;
                    if (tempDistance < distance)
                    {
                        hit = buffer[j];
                        distance = tempDistance;
                    }
                }
                
                for (var j = 0; j < hitCount; j++)
                {
                    // Get the collision normal based on the contact point
                    Vector3 collisionNormal = hit.normal;

                    // Apply a response to the rope point based on the collision normal
                    ropePoint.position += collisionNormal * COLLISION_FORCE;
                }
                
            }
        }

        void CalculatePointNormals()
        {
            for (int i = 0; i < segments - 1; i++)
            {
                ref RopePoint p0 = ref ropePoints[i];
                ref RopePoint p1 = ref ropePoints[i + 1];
                p0.normal = Vector3.Cross(Vector3.forward, (p1.position - p0.position).normalized);
                p0.right = (p1.position - p0.position).normalized;
            }

            ref var last2 = ref ropePoints[^2];
            ref var last = ref ropePoints[^1];
            last.normal = last2.normal;
            last.right = last2.right;
        }

        void CheckShouldSleep()
        {
            const float DISTANCE_THRESHOLD = 2.5f;
            const float VELOCITY_THRESHOLD = 0.01f;
            for (int i = 0; i < segments; i++)
            {
                ref var ropePoint = ref ropePoints[i];
                var pos = ropePoint.position;
                var viewportPoint = cam.WorldToViewportPoint(pos);
                var hasForce = ropePoint.velocity.magnitude > VELOCITY_THRESHOLD;
                isAwake = viewportPoint.x > -DISTANCE_THRESHOLD && viewportPoint.x < 1f + DISTANCE_THRESHOLD && viewportPoint.y > -DISTANCE_THRESHOLD && viewportPoint.y < 1f + DISTANCE_THRESHOLD;
                if (isAwake == false && hasForce) isAwake = true;
                if (isAwake) break;
            }
        }

        public ref RopePoint GetPoint(int index) => ref ropePoints[index];
        public void UpdatePoint(RopePoint newValue) => ropePoints[newValue.index] = newValue;

        public List<RopePoint> GetPointList()
        {
            var ropePointList = ListPool<RopePoint>.Get();
            for (int i = 0; i < segments; i++)
            {
                ropePointList.Add(ropePoints[i]);
            }
            return ropePointList;
        }

        public void GetPositionsNonAlloc(IList<Vector3> list)
        {
            for (int i = 0; i < segments; i++)
            {
                list.Add(ropePoints[i].position);
            }
        }

        public void GetPositionsNonAlloc(Vector3[] arr)
        {
            for (int i = 0; i < segments && i < arr.Length; i++)
            {
                arr[i] = ropePoints[i].position;
            }
        }

        public void ReleasePointList(List<RopePoint> ropePointList) => ListPool<RopePoint>.Release(ropePointList);

        public void AddOrUpdateConstraint(IRopeConstraint constraint)
        {
            if (constraints.ContainsKey(constraint.ropePointIndex) == false)
            {
                var list = new DynamicArray<IRopeConstraint>();
                list.Add() = constraint;
                constraints.Add(constraint.ropePointIndex, list);
                return;
            }
            
            var constraintType = constraint.GetType();
            var constraintList = constraints[constraint.ropePointIndex];
            bool updated = false;
            for (var i = 0; i < constraintList.Count; i++)
            {
                ref IRopeConstraint ropeConstraint = ref constraintList[i];
                if (ropeConstraint.GetType() == constraintType)
                {
                    constraintList[i] = constraint;
                    updated = true;
                    break;
                }
            }

            if (updated == false) constraintList.Add() = constraint;
        }

        public void RemoveConstraint(int pointIndex, Type constraintType)
        {
            if (constraints.ContainsKey(pointIndex) == false) return;

            var list = constraints[pointIndex];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].GetType() == constraintType)
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }

        public float GetCurrentLength()
        {
            var positions = ListPool<Vector3>.Get();
            for (int i = 0; i < segments; i++)
            {
                positions.Add(ropePoints[i].position);
            }
            float length = SplineMath.GetLength(positions, segments / 4);
            ListPool<Vector3>.Release(positions);
            return length;
        }

#if UNITY_EDITOR
        [SerializeField] bool showSegments;
        void OnDrawGizmos()
        {
            if (showSegments == false)
            {
                return;
            }

            if (Application.isPlaying == false)
            {
                var length = segments * segmentLength;
                var ropeEndPos = transform.position + creationDirection.normalized * length;
                XIV.Core.XIVDebug.DrawLine(transform.position, ropeEndPos, Color.green);
                return;
            }

            var positions = ListPool<Vector3>.Get();
            for (int i = 0; i < segments; i++)
            {
                ref var p = ref ropePoints[i];
                var pos = p.position;
                positions.Add(pos);
                XIV.Core.XIVDebug.DrawCircle(pos, 0.25f, Vector3.forward, Color.blue);
                XIV.Core.XIVDebug.DrawLine(pos, pos + p.right, Color.red);
                XIV.Core.XIVDebug.DrawLine(pos, pos + p.normal, Color.yellow);
            }
            XIV.Core.XIVDebug.DrawSpline(positions, Color.green, segments);
            ListPool<Vector3>.Release(positions);
        }
#endif
        public ref RopePoint GetClosestPoint(Vector3 position)
        {
            float distance = float.MaxValue;
            int selected = 0;

            for (int i = 0; i < segments; i++)
            {
                ref var current = ref ropePoints[i];
                var dis = Vector3.Distance(position, current.position);
                if (dis < distance)
                {
                    distance = dis;
                    selected = i;
                }
            }
            
            return ref ropePoints[selected];
        }

        public void AddForce(Vector3 position, Vector3 force)
        {
            ref var closestRopePoint = ref GetClosestPoint(position);
            for (int i = 0; i <= closestRopePoint.index; i++)
            {
                var forceMultiplier = i / (float)closestRopePoint.index;
                if (float.IsNaN(forceMultiplier) || float.IsInfinity(forceMultiplier)) forceMultiplier = 1f;
                ref var point = ref ropePoints[i];
                point.force += (force) * forceMultiplier;
            }

            for (int i = closestRopePoint.index + 1; i < segments; i++)
            {
                var forceMultiplier = (i - closestRopePoint.index) / (float)(segments - 1);
                if (float.IsNaN(forceMultiplier) || float.IsInfinity(forceMultiplier)) forceMultiplier = 1f;
                ref var point = ref ropePoints[i];
                point.force += (force) * forceMultiplier;
            }
        }
    }
}