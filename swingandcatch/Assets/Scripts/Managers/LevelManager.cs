using TheGame.SceneManagement;
using TheGame.ScriptableObjects.AudioManagement;
using TheGame.ScriptableObjects.Channels;
using UnityEngine;

namespace TheGame.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] LevelDataChannelSO levelDataLoadedChannel;
        [SerializeField] VoidChannelSO sceneActivatedChannel;
        [SerializeField] AudioPlayerSO levelMusicAudioPlayer;

        LevelData levelData;

        void OnEnable()
        {
            sceneActivatedChannel.Register(OnSceneActivated);
            levelDataLoadedChannel.Register(OnSceneListDataLoaded);
        }

        void OnDisable()
        {
            sceneActivatedChannel.Unregister(OnSceneActivated);
            levelDataLoadedChannel.Unregister(OnSceneListDataLoaded);
        }

        void OnSceneListDataLoaded(LevelData levelData)
        {
            this.levelData = levelData;
            this.levelData.SetLastPlayedLevel(gameObject.scene.buildIndex);
        }

        void OnSceneActivated()
        {
            levelMusicAudioPlayer.Play();
        }

        void OnDestroy()
        {
            levelMusicAudioPlayer.Stop();
        }
    }
}