using TheGame.SaveSystems;
using TheGame.ScriptableObjects.Channels;
using TheGame.ScriptableObjects.SceneManagement;
using UnityEngine;

namespace TheGame.SceneManagement
{
    public class LevelDataManager : MonoBehaviour, ISavable
    {
        [SerializeField] VoidChannelSO onSceneLoaded;
        [SerializeField] SceneListSO sceneListSO;
        [SerializeField] LevelDataChannelSO levelDataLoadedChannel;

        LevelData levelData;

        void Awake() => levelData = sceneListSO.GetSceneListData();
        void Start() => levelDataLoadedChannel.RaiseEvent(levelData);

        void OnEnable()
        {
            onSceneLoaded.Register(OnSceneLoaded);
        }

        void OnDisable()
        {
            onSceneLoaded.Unregister(OnSceneLoaded);
        }

        void OnSceneLoaded()
        {
            levelDataLoadedChannel.RaiseEvent(levelData);
        }

        object ISavable.GetSaveData()
        {
            return new SaveData
            {
                lastPlayedLevel = levelData.lastPlayedLevel,
            };
        }

        void ISavable.LoadSaveData(object data)
        {
            var saveData = (SaveData)data;
            levelData.SetLastPlayedLevel(saveData.lastPlayedLevel);
        }
        
        [System.Serializable]
        struct SaveData
        {
            public int lastPlayedLevel;
        }
    }
}