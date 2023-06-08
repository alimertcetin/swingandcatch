using System;
using UnityEngine;
using XIV.Core;

namespace TheGame.VerletRope
{
    [RequireComponent(typeof(Rope))]
    public class PositionConstraintMono : MonoBehaviour
    {
        [SerializeField] Vector3 localPosition;
        [SerializeField] int pointIndex;
        Rope rope;
        Vector3 previousPosition;

        void Awake()
        {
            previousPosition = transform.position;
            (rope = GetComponent<Rope>()).AddOrUpdateConstraint(new PositionConstraint(pointIndex, transform.TransformPoint(localPosition)));
        }

        void Update()
        {
            if (rope.isAwake == false) return;
            
            if (previousPosition != transform.position) rope.AddOrUpdateConstraint(new PositionConstraint(pointIndex, transform.TransformPoint(localPosition)));
            
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var pos = transform.TransformPoint(localPosition);
            XIVDebug.DrawCircle(pos, 0.25f, Vector3.forward, Color.magenta);
        }
#endif
    }
}