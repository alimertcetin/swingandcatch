using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XIV.Core.XIVMath;

namespace TheGame
{
    public class ParallaxPlane : MonoBehaviour
    {
        public float distanceFromCamera = 5f;
        public float movementSharpness = 10f;
        public Vector3 positionOffset;
        Camera cam;
        Material material;
        Vector3 camInitialPosition;

        void Awake()
        {
            cam = Camera.main;
            material = GetComponent<Renderer>().material;
            var camTransform = cam.transform;
            transform.position = camTransform.position + Vector3.forward * distanceFromCamera + positionOffset;
            transform.localScale = FrustumMath.GetFrustum(cam, distanceFromCamera);
            transform.SetParent(camTransform);
            camInitialPosition = camTransform.position;
        }

        void Update()
        {
            var camCurrentPosition = cam.transform.position;
            Vector3 diff = camCurrentPosition - camInitialPosition;
            var localScale = transform.localScale;
            var offsetX = diff.x / localScale.x;
            var offsetY = diff.y / localScale.y;

            int parallaxOffsetVectorID = ShaderConstants.ShaderGraphs_ParallaxBackgroundShader.ParallaxOffset_VectorID;
            var offsetVec = material.GetVector(parallaxOffsetVectorID);

            offsetX = Mathf.MoveTowards(offsetVec.x, offsetX, movementSharpness * Time.deltaTime);
            offsetY = Mathf.MoveTowards(offsetVec.y, offsetY, movementSharpness * Time.deltaTime);
            
            material.SetVector(parallaxOffsetVectorID, new Vector4(offsetX, offsetY, 0f, 0f));
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            var cam = Camera.main;
            transform.position = cam.transform.position + Vector3.forward * distanceFromCamera + positionOffset;
            transform.localScale = FrustumMath.GetFrustum(cam, distanceFromCamera);
        }
#endif
    }
}
