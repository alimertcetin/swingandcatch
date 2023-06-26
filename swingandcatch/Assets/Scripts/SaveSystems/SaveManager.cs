using System.Collections;
using TheGame.ScriptableObjects.Channels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGame.SaveSystems
{
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] IntChannelSO saveSceneDataChannel;
        [SerializeField] IntChannelSO loadSceneDataChannel;
        
        [SerializeField] VoidChannelSO onSaveCompletedChannel;
        [SerializeField] VoidChannelSO onLoadCompletedChannel;

        bool isSaving;
        bool isLoading;

        void OnEnable()
        {
            saveSceneDataChannel.Register(SaveSceneData);
            loadSceneDataChannel.Register(LoadSceneData);
        }

        void OnDisable()
        {
            saveSceneDataChannel.Unregister(SaveSceneData);
            loadSceneDataChannel.Unregister(LoadSceneData);
        }

        void SaveSceneData(int sceneIndex)
        {
            if (isSaving) return;
            var sceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
            StartCoroutine(WaitSave(sceneName));
        }

        void LoadSceneData(int sceneIndex)
        {
            if (isLoading) return;
            var sceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
            StartCoroutine(WaitLoad(sceneName));
        }

        IEnumerator WaitSave(string sceneName)
        {
            isSaving = true;
            yield return SaveSystem.SaveAsync(sceneName);
            onSaveCompletedChannel.RaiseEvent();
            isSaving = false;
        }

        IEnumerator WaitLoad(string sceneName)
        {
            isLoading = true;
            yield return SaveSystem.LoadAsync(sceneName);
            onLoadCompletedChannel.RaiseEvent();
            isLoading = false;
        }
    }
}