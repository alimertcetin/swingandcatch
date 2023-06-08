namespace TheGame.VerletRope
{
    public interface IRopeConstraint
    {
        public int ropePointIndex { get; }
        public void Apply(RopePoint[] ropePoints);
    }
}