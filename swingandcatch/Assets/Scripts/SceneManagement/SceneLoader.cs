using System.Collections;
using TheGame.ScriptableObjects.Channels;
using UnityEngine;
using UnityEngine.SceneManagement;
using XIV.Core.Utils;

namespace TheGame.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        // 0 = Initialization, 1 = PersistantManager, 2 = MainMenu
        
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
        [SerializeField] BoolChannelSO displayLoadingScreenChannel;
        [SerializeField] FloatChannelSO sceneLoadingProgressChannel;
        [SerializeField] VoidChannelSO newSceneLoadedChannel;
        [SerializeField] VoidChannelSO activateNewlyLoadedScene;
        
        AsyncOperation currentLoadingOperation;
        Timer sceneLoadTimer = new Timer(4f);
        
        int sceneToLoad;
        int sceneToUnload;
        bool isDisplayingLoadingScreen;
        bool activateImmediately;

        void OnEnable()
        {
            sceneLoadChannel.Register(LoadScene);
            activateNewlyLoadedScene.Register(ActivateNewScene);
        }

        void OnDisable()
        {
            sceneLoadChannel.Unregister(LoadScene);
            activateNewlyLoadedScene.Unregister(ActivateNewScene);
        }

        void LoadScene(SceneLoadOptions sceneLoadOptions)
        {
            if (currentLoadingOperation != null)
            {
                Debug.LogError("SceneLoading was requested while there is still ongoing loading operation");
            }

            isDisplayingLoadingScreen = sceneLoadOptions.displayLoadingScreen;
            activateImmediately = sceneLoadOptions.activateImmediately;
            displayLoadingScreenChannel.RaiseEvent(isDisplayingLoadingScreen);
            sceneToLoad = sceneLoadOptions.sceneToLoad;
            
            if (sceneLoadOptions.unloadActiveScene)
            {
                sceneToUnload = SceneManager.GetActiveScene().buildIndex;
                UnloadPreviousAndLoadNew();
                return;
            }
            
            LoadNewScene();
        }

        void UnloadPreviousAndLoadNew()
        {
            var unloadSceneOp = SceneManager.UnloadSceneAsync(sceneToUnload);
            unloadSceneOp.allowSceneActivation = false;
            unloadSceneOp.completed += (_) => LoadNewScene();
        }

        void LoadNewScene()
        {
            currentLoadingOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            currentLoadingOperation.allowSceneActivation = false;
            StartCoroutine(HandleAsyncLoading());
        }

        IEnumerator HandleAsyncLoading()
        {
            if (isDisplayingLoadingScreen)
            {
                while (sceneLoadTimer.IsDone == false)
                {
                    sceneLoadTimer.Update(Time.deltaTime);
                    sceneLoadingProgressChannel.RaiseEvent(currentLoadingOperation.progress * sceneLoadTimer.NormalizedTime);
                    yield return null;
                }
            }

            while (currentLoadingOperation.progress < 0.9f)
            {
                yield return null;
            }
            
            sceneLoadingProgressChannel.RaiseEvent(1f);
            currentLoadingOperation.allowSceneActivation = true;
            yield return null;
            
            currentLoadingOperation = null;
            sceneLoadTimer.Restart();
            if (activateImmediately)
            {
                ActivateNewScene();
                yield break;
            }

            newSceneLoadedChannel.RaiseEvent();
        }

        void ActivateNewScene()
        {
            LightProbes.Tetrahedralize();
            displayLoadingScreenChannel.RaiseEvent(false);
            Debug.Log("SceneManager.GetActiveScene() BEFORE= " + SceneManager.GetActiveScene().name);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneToLoad));
            Debug.Log("SceneManager.GetActiveScene() AFTER = " + SceneManager.GetActiveScene().name);
        }
    }
}
