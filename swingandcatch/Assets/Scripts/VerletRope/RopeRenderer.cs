using System;
using UnityEngine;
using UnityEngine.Pool;

namespace TheGame.VerletRope
{
    public class RopeRenderer : MonoBehaviour
    {
        [SerializeField] Rope rope;
        [SerializeField] LineRenderer lineRenderer;

        Vector3[] positions;

        void Start()
        {
            positions = new Vector3[rope.Segments];
            lineRenderer.enabled = true;
            RenderRope();
        }

        void Update()
        {
            if (rope.isAwake == false) return;
            RenderRope();
        }

        void RenderRope()
        {
            int segments = rope.Segments;
            if (positions.Length != segments) Array.Resize(ref positions, segments);
            rope.GetPositionsNonAlloc(positions);

            if (lineRenderer.positionCount != segments) lineRenderer.positionCount = segments;
            
            lineRenderer.SetPositions(positions);
        }
    }
}