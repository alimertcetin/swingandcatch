using System;
using TheGame.SaveSystems;
using TheGame.ScriptableObjects.Channels;
using UnityEngine;

namespace TheGame.SettingSystems
{
    public class SettingsManager : MonoBehaviour, ISavable
    {
        [SerializeField] VoidChannelSO onSceneLoaded;
        [SerializeField] SettingsChannelSO settingsLoaded;
        Settings settings;

        void Awake()
        {
            settings = new Settings();
            settings.Initialize();
            SetFrameRate(60);
        }

        void Start()
        {
            settingsLoaded.RaiseEvent(settings);
        }

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
            settingsLoaded.RaiseEvent(settings);
        }

        void SetFrameRate(int frameRate)
        {
            Application.targetFrameRate = frameRate;
        }

        object ISavable.GetSaveData()
        {
            return ((ISavable)settings).GetSaveData();
        }

        void ISavable.LoadSaveData(object data)
        {
            ((ISavable)settings).LoadSaveData(data);
        }
    }
}