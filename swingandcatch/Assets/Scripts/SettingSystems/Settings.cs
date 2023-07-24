using TheGame.SaveSystems;
using XIV.Core.Collections;

namespace TheGame.SettingSystems
{
    public class Settings : ISavable
    {
        DynamicArray<ISettingsListener> settingsListeners = new DynamicArray<ISettingsListener>();
        ISettingParameterContainer[] settingParameterContainers;

        public void Initialize()
        {
            var graphicSettingsParameterContainer = new GraphicSettingsParameterContainer();
            graphicSettingsParameterContainer.Initialize();
            var audioSettingsParameterContainer = new AudioSettingsParameterContainer();
            audioSettingsParameterContainer.Initialize();
            settingParameterContainers = new ISettingParameterContainer[]
            {
                graphicSettingsParameterContainer,
                audioSettingsParameterContainer,
            };
        }

        public void AddListener(ISettingsListener listener)
        {
            if (settingsListeners.Contains(ref listener)) return;
            settingsListeners.Add() = listener;
        }

        public bool RemoveListener(ISettingsListener listener)
        {
            return settingsListeners.Remove(ref listener);
        }

        public bool SetParameter(SettingParameterType parameterType, int parameterNameHash, object newValue)
        {
            var container = GetParameterContainer(parameterType);
            var success = container.SetParameter(parameterNameHash, newValue);
            if (success) InformListeners(container.GetParameter(parameterNameHash));
            return success;
        }

        public SettingParameter GetParameter(SettingParameterType parameterType, int parameterNameHash)
        {
            var container = GetParameterContainer(parameterType);
            return container.GetParameter(parameterNameHash);
        }

        void InformListeners(SettingParameter settingParameter)
        {
            int listenerCount = settingsListeners.Count;
            for (int i = 0; i < listenerCount; i++)
            {
                settingsListeners[i].OnSettingsChanged(settingParameter);
            }
        }

        ISettingParameterContainer GetParameterContainer(SettingParameterType parameterType)
        {
            int length = settingParameterContainers.Length;
            for (int i = 0; i < length; i++)
            {
                if (settingParameterContainers[i].GetParameterType() == parameterType) return settingParameterContainers[i];
            }

            return default;
        }

        #region Save

        [System.Serializable]
        struct SavableData
        {
            public SettingParameterType parameterType;
            public object data;
        }

        [System.Serializable]
        struct SaveData
        {
            public SavableData[] savableDatas;
        }

        object ISavable.GetSaveData()
        {
            int length = settingParameterContainers.Length;
            var savableDatas = new SavableData[length];
            for (int i = 0; i < length; i++)
            {
                savableDatas[i].parameterType = settingParameterContainers[i].GetParameterType();
                savableDatas[i].data = settingParameterContainers[i].GetSaveData();
            }

            return new SaveData
            {
                savableDatas = savableDatas
            };
        }

        void ISavable.LoadSaveData(object data)
        {
            var saveData = (SaveData)data;
            int length = saveData.savableDatas.Length;
            int parameterContainerLength = settingParameterContainers.Length;
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < parameterContainerLength; j++)
                {
                    var settingParameterContainer = settingParameterContainers[j];
                    if (saveData.savableDatas[i].parameterType == settingParameterContainer.GetParameterType())
                    {
                        settingParameterContainer.LoadSaveData(saveData.savableDatas[i].data);
                    }
                }
            }
        }

        #endregion
        
    }
}