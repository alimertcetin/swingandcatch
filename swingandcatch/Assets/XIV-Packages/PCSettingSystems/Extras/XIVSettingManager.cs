using TheGame.ScriptableObjects.Channels;
using UnityEngine;
using XIV.Core;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.ScriptableObjects.Channels;

namespace XIV_Packages.PCSettingSystems.Extras
{
    public class XIVSettingManager : MonoBehaviour
    {
        [SerializeField] XIVSettingChannelSO settingLoadedChannel;
        [SerializeField] VoidChannelSO onSceneReadyChannel;
        [SerializeField, DisplayWithoutEdit] SettingManager[] settingManagers;

        XIVSettings settings;

        void Awake()
        {
            settings = new XIVSettings();
            int length = settingManagers.Length;
            for (int i = 0;  i < length;  i++)
            {
                var settingManager = settingManagers[i];
                settingManager.InitializeContainer();
                settings.AddContainer(settingManager.GetContainer());
            }
        }

        void Start()
        {
            settingLoadedChannel.RaiseEvent(settings);
        }

        void OnEnable()
        {
            onSceneReadyChannel.Register(OnSceneLoaded);
        }

        void OnDisable()
        {
            onSceneReadyChannel.Unregister(OnSceneLoaded);
        }

        void OnSceneLoaded()
        {
            settingLoadedChannel.RaiseEvent(settings);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            settingManagers = GetComponentsInChildren<SettingManager>();
        }
#endif

    }
}