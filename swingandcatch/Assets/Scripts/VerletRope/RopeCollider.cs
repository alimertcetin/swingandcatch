using UnityEngine;
using UnityEngine.Pool;

namespace TheGame.VerletRope
{
    public class RopeCollider : MonoBehaviour
    {
        [SerializeField] Rope rope;
        [SerializeField] PolygonCollider2D polygonCollider2D;
        [SerializeField] float thickness;

        void Start()
        {
            UpdateCollider();
        }

        void Update()
        {
            UpdateCollider();
        }

        void UpdateCollider()
        {
            
            var ropePoints = rope.GetPointList();
            var vertices = ListPool<Vector2>.Get();
            
            var transformPosition = transform.position;
            for (int i = 0; i < rope.Segments; i++)
            {
                var ropePoint = ropePoints[i];
                var vertex = ropePoint.position + ropePoint.normal * thickness;
                vertex -= transformPosition;
                vertices.Add(vertex);
            }

            for (int i = rope.Segments - 1; i >= 0; i--)
            {
                var ropePoint = ropePoints[i];
                var vertex = ropePoint.position - ropePoint.normal * thickness;
                vertex -= transformPosition;
                vertices.Add(vertex);
            }

            polygonCollider2D.SetPath(0, vertices);
            // polygonCollider2D.points = vertices.ToArray();
            ListPool<Vector2>.Release(vertices);
            rope.ReleasePointList(ropePoints);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (rope == null) rope = GetComponent<Rope>();
            if (polygonCollider2D == null) polygonCollider2D = GetComponent<PolygonCollider2D>();
        }
#endif
    }
}