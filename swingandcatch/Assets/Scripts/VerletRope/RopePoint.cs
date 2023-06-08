using UnityEngine;

namespace TheGame.VerletRope
{
    public struct RopePoint
    {
        public int index;
        public Vector3 previousPosition;
        public Vector3 position;
        public Vector3 velocity => position - previousPosition;
        public Vector3 force;
        public Vector3 normal;
        public Vector3 right;

        public RopePoint(RopePoint other)
        {
            this.index = other.index;
            this.position = other.position;
            this.previousPosition = other.previousPosition;
            this.force = other.force;
            this.normal = other.normal;
            this.right = other.right;
        }
    }
}