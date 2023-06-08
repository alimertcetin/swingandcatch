using UnityEngine;

namespace TheGame.VerletRope
{
    public readonly struct PositionConstraint : IRopeConstraint
    {
        public int ropePointIndex { get; }
        public Vector3 position { get; }

        public PositionConstraint(int ropePointIndex, Vector3 position)
        {
            this.ropePointIndex = ropePointIndex;
            this.position = position;
        }
        
        void IRopeConstraint.Apply(RopePoint[] ropePoints)
        {
            ropePoints[ropePointIndex].position = position;
        }
    }
}