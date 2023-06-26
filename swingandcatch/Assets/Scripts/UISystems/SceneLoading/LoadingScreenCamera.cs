using TheGame.ScriptableObjects.Channels;
using UnityEngine;

namespace TheGame.UISystems.SceneLoading
{
    [RequireComponent(typeof(Camera))]
    public class LoadingScreenCamera : MonoBehaviour
    {
        [SerializeField] BoolChannelSO activateLoadingScreenCamera;

        Camera cam;
        
        void Awake() => cam = GetComponent<Camera>();
        void OnEnable() => activateLoadingScreenCamera.Register(OnActivateLoadingScreenCamera);
        void OnDisable() => activateLoadingScreenCamera.Unregister(OnActivateLoadingScreenCamera);
        void OnActivateLoadingScreenCamera(bool value) => cam.enabled = value;
    }
}