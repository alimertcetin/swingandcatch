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
        static GUIStyle guiStyle;
        void OnDrawGizmosSelected()
        {
            var pos = transform.TransformPoint(localPosition);
            XIVDebug.DrawCircle(pos, 0.25f, Vector3.forward, Color.magenta);
            GUIStyle style;
            if (guiStyle == null)
            {
                guiStyle = new GUIStyle(GUI.skin.label);
                guiStyle.normal.textColor = Color.red;
                guiStyle.fontStyle = FontStyle.Bold;
                style = guiStyle;
            }
            else
            {
                style = guiStyle;
            }
            UnityEditor.Handles.Label(transform.position, nameof(PositionConstraintMono), style);
        }
#endif
    }
}