using System.Collections;
using TheGame.ScriptableObjects.Channels;
using TheGame.ScriptableObjects.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGame.SceneManagement
{
    public class SceneInitializer : MonoBehaviour
    {
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
        [SerializeField] SceneListSO sceneListSO;
        
        void Awake()
        {
            StartCoroutine(LoadPersistantScene());
        }

        IEnumerator LoadPersistantScene()
        {
            var asyncOp = SceneManager.LoadSceneAsync(sceneListSO.persistantManagerSceneIndex, LoadSceneMode.Additive);
            asyncOp.allowSceneActivation = true;
            while (asyncOp.progress < 0.9f)
            {
                yield return null;
            }

            // SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneListSO.persistantManagerSceneIndex));
            
            var options = SceneLoadOptions.MenuLoad(sceneListSO.mainMenuSceneIndex);
            options.loadingScreenType = LoadingScreenType.None;
            sceneLoadChannel.RaiseEvent(options);
        }
    }
}
