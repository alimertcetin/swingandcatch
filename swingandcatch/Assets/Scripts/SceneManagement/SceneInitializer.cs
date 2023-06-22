using TheGame.Data;
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
            var asyncOp = SceneManager.LoadSceneAsync(GameData.SceneData.PERSISTANT_MANAGER, LoadSceneMode.Additive);
            asyncOp.allowSceneActivation = true;
            asyncOp.completed += (op) =>
            {
                var options = SceneLoadOptions.MenuLoad(GameData.SceneData.MAIN_MENU);
                options.loadingScreenType = LoadingScreenType.None;
                sceneLoadChannel.RaiseEvent(options);
                SceneManager.UnloadSceneAsync(0);
            };
        }
    }
}
