using System;
using TheGame.AudioManagement;
using TheGame.SaveSystems;
using TheGame.ScriptableObjects.Channels;
using TheGame.ScriptableObjects.SceneManagement;
using TheGame.UISystems;
using TheGame.UISystems.Core;
using UnityEngine;

namespace TheGame.SceneManagement
{
    public class LevelManager : MonoBehaviour, ISavable
    {
        [SerializeField] TransformChannelSO playerReachedEndChannelSO;
        [SerializeField] VoidChannelSO showWinUIChannel;
        [SerializeField] SceneListSO sceneListSO;
        [SerializeField] VoidChannelSO sceneActivatedChannel;
        [SerializeField] AudioPlayOptionsChannelSO audioPlayOptionsChannel;
        [SerializeField] AudioClip levelMusic;
        
        int currentLevel;

        void Awake()
        {
            currentLevel = gameObject.scene.buildIndex;
            sceneListSO.lastPlayedLevel = currentLevel;
        }

        void OnEnable()
        {
            playerReachedEndChannelSO.Register(OnLevelCompleted);
            sceneActivatedChannel.Register(OnSceneActivated);
        }

        void OnDisable()
        {
            playerReachedEndChannelSO.Unregister(OnLevelCompleted);
            sceneActivatedChannel.Unregister(OnSceneActivated);
        }

        void OnDestroy()
        {
            audioPlayOptionsChannel.RaiseEvent(AudioPlayOptions.MusicPlayOptions(null));
        }

        void OnSceneActivated()
        {
            audioPlayOptionsChannel.RaiseEvent(AudioPlayOptions.MusicPlayOptions(levelMusic));
        }

        void OnLevelCompleted(Transform playerTransform)
        {
            var playerWinUI = UISystem.GetUI<PlayerWinUI>();
            
            if (sceneListSO.TryGetNextLevel(currentLevel, out var nextLevelBuildIndex))
            {
                currentLevel = nextLevelBuildIndex;
                playerWinUI.nextLevel = currentLevel;
            }
            else
            {
                playerWinUI.nextLevel = -1;
            }

            showWinUIChannel.RaiseEvent();
        }

        object ISavable.GetSaveData()
        {
            return new SaveData
            {
                lastPlayedLevel = sceneListSO.lastPlayedLevel,
            };
        }

        void ISavable.LoadSaveData(object data)
        {
            var saveData = (SaveData)data;
            this.sceneListSO.lastPlayedLevel = saveData.lastPlayedLevel;
        }
        
        [System.Serializable]
        struct SaveData
        {
            public int lastPlayedLevel;
        }
    }
}