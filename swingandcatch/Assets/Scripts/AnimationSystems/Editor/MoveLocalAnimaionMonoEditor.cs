using System;
using UnityEditor;
using UnityEngine;
using XIV.XIVEditor;

namespace TheGame.AnimationSystems.Editor
{
    [CustomEditor(typeof(MoveLocalAnimationMono)), CanEditMultipleObjects]
    public class MoveLocalAnimaionMonoEditor : XIVDefaulEditor
    {
        SerializedObject so;
        SerializedProperty spAxis;
        bool inEditMode;
        
        void OnEnable()
        {
            so = serializedObject;
            spAxis = so.FindProperty("axis");
        }

        void OnDisable()
        {
            inEditMode = false;
            Tools.hidden = false;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying) return;
            
            if (GUILayout.Button("Reset"))
            {
                so.Update();
                spAxis.vector3Value = Vector3.up;
                so.FindProperty("moveDistance").floatValue = 2f;
                so.FindProperty("moveSpeed").floatValue = 1f;
                so.ApplyModifiedProperties();
            }

            var buttontext = inEditMode ? "Exit Edit Mode" : "Enter Edit Mode";
            if (GUILayout.Button(buttontext))
            {
                inEditMode = !inEditMode;
                Tools.hidden = inEditMode;
            }
        }

        void OnSceneGUI()
        {
            if (inEditMode == false) return;

            so.Update();
            var moveLocalAnimationMono = ((MoveLocalAnimationMono)target);
            var transform = moveLocalAnimationMono.transform;

            Vector3 position = transform.position;
            var axisNormalized = transform.TransformDirection(spAxis.vector3Value.normalized);
            var movementEnd = position + axisNormalized;
            var newMovementEnd = Handles.DoPositionHandle(movementEnd, Quaternion.identity);

            var dir = (newMovementEnd - movementEnd);
            if (dir.magnitude < 0.00001f) return;

            var newAxis = dir.normalized;
            spAxis.vector3Value = transform.InverseTransformDirection(newAxis);
            so.ApplyModifiedProperties();
        }
    }
}