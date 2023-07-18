using System.Collections;
using TheGame.ScriptableObjects.Channels;
using UnityEngine;
using UnityEngine.SceneManagement;
using XIV.Core.Utils;

namespace TheGame.SceneManagement
{
    // TODO : SceneLoader -> SOLID?! DRY?! KISS?!
    public class SceneLoader : MonoBehaviour
    {
        [Header("Listening")]
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
        [SerializeField] VoidChannelSO activateNewlyLoadedScene;
        
        [SerializeField] VoidChannelSO onSaveCompletedChannel;
        [SerializeField] VoidChannelSO onLoadCompletedChannel;
        
        [Header("Broadcasting")]
        [SerializeField] SceneLoadChannelSO displayLoadingScreenChannel;
        [SerializeField] VoidChannelSO stopDisplayingLoadingScreenChannel;
        [SerializeField] FloatChannelSO sceneLoadingProgressChannel;
        [SerializeField] VoidChannelSO newSceneLoadedChannel;
        [SerializeField] VoidChannelSO sceneActivatedChannel;
        
        [SerializeField] IntChannelSO saveSceneDataChannel;
        [SerializeField] IntChannelSO loadSceneDataChannel;

        AsyncOperation currentLoadingOperation;
        Timer sceneLoadTimer = new Timer(2f);
        
        int sceneToUnload;
        SceneLoadOptions currentLoadingOptions;

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
                Debug.LogError("SceneLoading was requested while there is still ongoing loading operation. Build index of requested scene : " + sceneLoadOptions.sceneToLoad);
                return;
            }

            currentLoadingOptions = sceneLoadOptions;
            displayLoadingScreenChannel.RaiseEvent(currentLoadingOptions);
            sceneToUnload = SceneManager.GetActiveScene().buildIndex;
            
            onSaveCompletedChannel.Register(OnSaveCompleted); // wait for save completed response from save system
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
            currentLoadingOperation = SceneManager.LoadSceneAsync(currentLoadingOptions.sceneToLoad, LoadSceneMode.Additive);
            currentLoadingOperation.allowSceneActivation = false;
            StartCoroutine(HandleAsyncLoading());
        }

        IEnumerator HandleAsyncLoading()
        {
            yield return WaitSceneLoading();

            onLoadCompletedChannel.Register(OnLoadCompleted); // Wait for load completed response from save system
            loadSceneDataChannel.RaiseEvent(currentLoadingOptions.sceneToLoad);

            // On save loaded
            void OnLoadCompleted()
            {
                onLoadCompletedChannel.Unregister(OnLoadCompleted);
                currentLoadingOperation = null;
                if (currentLoadingOptions.activateImmediately) ActivateNewScene();
                
                newSceneLoadedChannel.RaiseEvent();
            }
            
        }

        IEnumerator WaitSceneLoading()
        {
            if (currentLoadingOptions.loadingScreenType != LoadingScreenType.None)
            {
                sceneLoadTimer.Restart(currentLoadingOptions.loadingScreenType == LoadingScreenType.MenuLoading ? 1.5f : 3f);
                EasingFunction.Function easing = currentLoadingOptions.loadingScreenType == LoadingScreenType.MenuLoading ? EasingFunction.Linear : EasingFunction.SmoothStop3;

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

            currentLoadingOperation.allowSceneActivation = true;
            while (currentLoadingOperation.isDone == false)
            {
                sceneLoadingProgressChannel.RaiseEvent(currentLoadingOperation.progress);
                yield return null;
            }

            sceneLoadingProgressChannel.RaiseEvent(1f);
        }

        void ActivateNewScene()
        {
            stopDisplayingLoadingScreenChannel.RaiseEvent();
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentLoadingOptions.sceneToLoad));
            sceneActivatedChannel.RaiseEvent();
        }
    }
}
