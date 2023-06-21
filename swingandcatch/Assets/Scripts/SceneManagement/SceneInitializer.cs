using TheGame.ScriptableObjects.Channels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGame.SceneManagement
{
    public class SceneInitializer : MonoBehaviour
    {
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
        
        void Awake()
        {
            var asyncOp = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            asyncOp.allowSceneActivation = true;
            asyncOp.completed += (op) =>
            {
                sceneLoadChannel.RaiseEvent(new SceneLoadOptions() { displayLoadingScreen = false, sceneToLoad = 2, unloadActiveScene = false, activateImmediately = true });
                SceneManager.UnloadSceneAsync(0);
            };
        }
    }
}
