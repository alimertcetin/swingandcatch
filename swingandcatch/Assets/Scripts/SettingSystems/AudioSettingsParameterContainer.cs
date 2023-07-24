using TheGame.SaveSystems;

namespace TheGame.SettingSystems
{
    public class AudioSettingsParameterContainer : ISettingParameterContainer
    {
        public const string MASTER_VOLUME = AudioMixerConstants.DefaultMixer.Parameters.MasterVolume;
        public static readonly int masterVolumeHash = MASTER_VOLUME.GetHashCode();
        
        public const string MUSIC_VOLUME = AudioMixerConstants.DefaultMixer.Parameters.MusicVolume;
        public static readonly int musicVolumeHash = MUSIC_VOLUME.GetHashCode();
        
        public const string EFFECTS_VOLUME = AudioMixerConstants.DefaultMixer.Parameters.EffectsVolume;
        public static readonly int effectsVolumeHash = EFFECTS_VOLUME.GetHashCode();

        SettingParameter[] parameters;

        public void Initialize()
        {
            parameters = new SettingParameter[]
            {
                new SettingParameter(SettingParameterType.Audio, MASTER_VOLUME, 0.5f), // Slider
                new SettingParameter(SettingParameterType.Audio, MUSIC_VOLUME, 0.5f), // Slider
                new SettingParameter(SettingParameterType.Audio, EFFECTS_VOLUME, 0.5f), // Slider
            };
        }

        public SettingParameterType GetParameterType() => SettingParameterType.Audio;

        public bool SetParameter(int parameterNameHash, object newValue)
        {
            int index = IndexOf(parameterNameHash);
            if (index < 0) return false;
            ChangeSetting(ref parameters[index], newValue);
            return true;
        }

        public SettingParameter GetParameter(int parameterNameHash)
        {
            return parameters[IndexOf(parameterNameHash)];
        }

        void ChangeSetting(ref SettingParameter setting, object newValue)
        {
            setting.value = newValue;
        }

        int IndexOf(int nameHash)
        {
            int length = parameters.Length;
            for (int i = 0; i < length; i++)
            {
                if (parameters[i].nameHash == nameHash) return i;
            }

            return -1;
        }

        [System.Serializable]
        struct SaveData
        {
            public float masterVolume;
            public float musicVolume;
            public float effectVolume;
        }

        object ISavable.GetSaveData()
        {
            return new SaveData
            {
                masterVolume = GetParameter(masterVolumeHash).ReadValue<float>(), 
                musicVolume = GetParameter(musicVolumeHash).ReadValue<float>(), 
                effectVolume = GetParameter(effectsVolumeHash).ReadValue<float>(),
            };
        }

        void ISavable.LoadSaveData(object data)
        {
            var saveData = (SaveData)data;
            SetParameter(masterVolumeHash, saveData.masterVolume);
            SetParameter(musicVolumeHash, saveData.musicVolume);
            SetParameter(effectsVolumeHash, saveData.effectVolume);
        }
    }
}