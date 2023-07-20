using TheGame.ScriptableObjects.Channels;
using UnityEngine;

namespace TheGame.UISystems.SceneLoading
{
    public class LoadingScreenCamera : MonoBehaviour
    {
        [SerializeField] BoolChannelSO activateLoadingScreenCamera;

        [SerializeField] Camera[] cams;
        
        void OnEnable() => activateLoadingScreenCamera.Register(OnActivateLoadingScreenCamera);
        void OnDisable() => activateLoadingScreenCamera.Unregister(OnActivateLoadingScreenCamera);
        void OnActivateLoadingScreenCamera(bool value)
        {
            int length = cams.Length;
            for (int i = 0; i < length; i++)
            {
                cams[i].enabled = value;
            }
        }
    }
}