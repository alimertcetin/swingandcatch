using UnityEngine;

namespace TheGame.VerletRope
{
    public readonly struct DistanceConstraint : IRopeConstraint
    {
        public int ropePointIndex { get; }
        public float distance { get; }
        public int correctionSteps { get; }

        public DistanceConstraint(int ropePointIndex, int correctionSteps, float distance)
        {
            this.ropePointIndex = ropePointIndex;
            this.distance = distance;
            this.correctionSteps = correctionSteps;
        }
        
        void IRopeConstraint.Apply(RopePoint[] ropePoints)
        {
            if (ropePointIndex == 0) return;
            for (int i = 0; i < correctionSteps; i++)
            {
                ref RopePoint p0 = ref ropePoints[ropePointIndex - 1];
                ref RopePoint p1 = ref ropePoints[ropePointIndex];

                float dist = Vector3.Distance(p0.position, p1.position);
                float error = Mathf.Abs(dist - distance);

                Vector3 dir = (dist < distance ? p0.position - p1.position : p1.position - p0.position).normalized;
                Vector3 correction = dir * (error * 0.5f);

                p0.position += correction;
                p1.position -= correction;
            }
        }
    }
}