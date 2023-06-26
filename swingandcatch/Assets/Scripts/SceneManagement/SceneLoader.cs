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
        
        [SerializeField] IntChannelSO saveSceneDataChannel;
        [SerializeField] IntChannelSO loadSceneDataChannel;
        
        [SerializeField] VoidChannelSO onSaveCompletedChannel;
        [SerializeField] VoidChannelSO onLoadCompletedChannel;
        
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
            sceneToUnload = SceneManager.GetActiveScene().buildIndex;
            
            onSaveCompletedChannel.Register(OnSaveCompleted);
            saveSceneDataChannel.RaiseEvent(sceneToUnload);

            void OnSaveCompleted()
            {
                onSaveCompletedChannel.Unregister(OnSaveCompleted);
            
                if (sceneLoadOptions.unloadActiveScene)
                {
                    UnloadPreviousAndLoadNew();
                    return;
                }
            
                LoadNewScene();
            }
            
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
                sceneLoadTimer.Restart(loadingScreenType == LoadingScreenType.MenuLoading ? 1.5f : 3f);
                EasingFunction.Function easing = loadingScreenType == LoadingScreenType.MenuLoading ? EasingFunction.Linear : EasingFunction.SmoothStop3;
                
                while (sceneLoadTimer.IsDone == false)
                {
                    sceneLoadTimer.Update(Time.deltaTime);
                    var t = easing(0f, 1f, sceneLoadTimer.NormalizedTime);
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
            
            loadSceneDataChannel.RaiseEvent(sceneToLoad);
            onLoadCompletedChannel.Register(OnLoadCompleted);

            void OnLoadCompleted()
            {
                onLoadCompletedChannel.Unregister(OnLoadCompleted);
                
                currentLoadingOperation = null;
                if (activateImmediately)
                {
                    ActivateNewScene();
                    return;
                }

                newSceneLoadedChannel.RaiseEvent();
            }
            
        }

        void ActivateNewScene()
        {
            stopDisplayingLoadingScreenChannel.RaiseEvent();
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneToLoad));
        }
    }
}
