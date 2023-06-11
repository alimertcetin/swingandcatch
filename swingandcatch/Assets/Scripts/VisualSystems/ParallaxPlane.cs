using UnityEngine;
using XIV.Core.XIVMath;

namespace TheGame.VisualSystems
{
    public class ParallaxPlane : MonoBehaviour
    {
        public float distanceFromCamera = 5f;
        public float movementSharpness = 10f;
        public Vector3 positionOffset;
        Camera cam;
        Material material;
        Vector3 camInitialPosition;
        Vector4 offsetVec;
        Vector3 localScale;

        void Awake()
        {
            cam = Camera.main;
            material = GetComponent<Renderer>().material;
            var camTransform = cam.transform;
            transform.position = camTransform.position + Vector3.forward * distanceFromCamera + positionOffset;
            transform.localScale = FrustumMath.GetFrustum(cam, distanceFromCamera);
            transform.SetParent(camTransform);
            camInitialPosition = camTransform.position;
            localScale = transform.localScale;
        }

        void Update()
        {
            Vector3 diff = cam.transform.position - camInitialPosition;
            if (diff.sqrMagnitude < Mathf.Epsilon) return;
            
            var offsetX = diff.x / localScale.x;
            var offsetY = diff.y / localScale.y;
            
            offsetVec.x = Mathf.MoveTowards(offsetVec.x, offsetX, movementSharpness * Time.deltaTime);
            offsetVec.y = Mathf.MoveTowards(offsetVec.y, offsetY, movementSharpness * Time.deltaTime);
            
            material.SetVector(ShaderConstants.ShaderGraphs_ParallaxBackgroundShader.ParallaxOffset_VectorID, offsetVec);
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
