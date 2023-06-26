using System;
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
        int currentLevel;

        void Awake()
        {
            currentLevel = gameObject.scene.buildIndex;
        }

        void OnEnable()
        {
            playerReachedEndChannelSO.Register(OnLevelCompleted);
        }

        void OnDisable()
        {
            playerReachedEndChannelSO.Unregister(OnLevelCompleted);
        }

        void OnLevelCompleted(Transform playerTransform)
        {
            var playerWinUI = UISystem.GetUI<PlayerWinUI>();
            
            if (sceneListSO.TryGetNextLevel(currentLevel, out var nextLevelBuildIndex))
            {
                sceneListSO.lastPlayedLevel = currentLevel;
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