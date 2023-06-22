using System.Collections;
using TheGame.ScriptableObjects.Channels;
using UnityEngine;
using UnityEngine.SceneManagement;
using XIV.Core.Utils;

namespace TheGame.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
        [SerializeField] SceneLoadChannelSO displayLoadingScreenChannel;
        [SerializeField] VoidChannelSO stopDisplayingLoadingScreenChannel;
        [SerializeField] FloatChannelSO sceneLoadingProgressChannel;
        [SerializeField] VoidChannelSO newSceneLoadedChannel;
        [SerializeField] VoidChannelSO activateNewlyLoadedScene;
        
        AsyncOperation currentLoadingOperation;
        Timer sceneLoadTimer = new Timer(2f);
        
        int sceneToLoad;
        int sceneToUnload;
        LoadingScreenType loadingScreenType;
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

            loadingScreenType = sceneLoadOptions.loadingScreenType;
            activateImmediately = sceneLoadOptions.activateImmediately;
            displayLoadingScreenChannel.RaiseEvent(sceneLoadOptions);
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
            if (loadingScreenType != LoadingScreenType.None)
            {
                while (sceneLoadTimer.IsDone == false)
                {
                    sceneLoadTimer.Update(Time.deltaTime);
                    var t = EasingFunction.SmoothStop3(sceneLoadTimer.NormalizedTime);
                    sceneLoadingProgressChannel.RaiseEvent(currentLoadingOperation.progress * t);
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
            stopDisplayingLoadingScreenChannel.RaiseEvent();
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneToLoad));
        }
    }
}
