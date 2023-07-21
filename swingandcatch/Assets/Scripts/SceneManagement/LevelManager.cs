using TheGame.ScriptableObjects.Channels;
using TheGame.ScriptableObjects.SceneManagement;
using TheGame.UISystems;
using TheGame.UISystems.Core;
using UnityEngine;

namespace TheGame.SceneManagement
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] LevelDataChannelSO levelDataLoadedChannel;
        [SerializeField] TransformChannelSO playerReachedEndChannelSO;
        [SerializeField] VoidChannelSO showWinUIChannel;
        [SerializeField] VoidChannelSO sceneActivatedChannel;
        [SerializeField] AudioPlayerSO levelMusicAudioPlayer;

        LevelData levelData;

        void OnEnable()
        {
            playerReachedEndChannelSO.Register(OnLevelCompleted);
            sceneActivatedChannel.Register(OnSceneActivated);
            levelDataLoadedChannel.Register(OnSceneListDataLoaded);
        }

        void OnDisable()
        {
            playerReachedEndChannelSO.Unregister(OnLevelCompleted);
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

        void OnLevelCompleted(Transform playerTransform)
        {
            var playerWinUI = UISystem.GetUI<PlayerWinUI>();
            playerWinUI.nextLevel = levelData == null ? -1 : levelData.TryGetNextLevel(gameObject.scene.buildIndex, out var nextLevelBuildIndex) ? nextLevelBuildIndex : -1;
            
            showWinUIChannel.RaiseEvent();
        }
    }
}