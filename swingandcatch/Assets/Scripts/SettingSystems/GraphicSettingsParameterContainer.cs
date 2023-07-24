using TheGame.SaveSystems;
using UnityEngine;

namespace TheGame.SettingSystems
{
    public class GraphicSettingsParameterContainer : ISettingParameterContainer
    {
        public const string PRESET = "Preset";
        public static readonly int presetHash = PRESET.GetHashCode();
        
        public const string RESOLUTION = "Resoulition";
        public static readonly int resolutionHash = RESOLUTION.GetHashCode();
        
        public const string DISPLAY_TYPE = "DisplayType";
        public static readonly int displayTypeHash = DISPLAY_TYPE.GetHashCode();
        
        public const string VSYNC = "Vsync";
        public static readonly int vsyncHash = VSYNC.GetHashCode();
        
        public const string ANTI_ALIAS = "AntiAlias";
        public static readonly int antiAliasHash = ANTI_ALIAS.GetHashCode();
        
        public const string BRIGHTNESS = "Brightness";
        public static readonly int brightnessHash = BRIGHTNESS.GetHashCode();
        
        public const string SHADOW_QUALITY = "ShadowQuality";
        public static readonly int shadowQualityHash = SHADOW_QUALITY.GetHashCode();
        
        public const string TEXTURE_QUALITY = "TextureQuality";
        public static readonly int textureQualityHash = TEXTURE_QUALITY.GetHashCode();

        const SettingParameterType SETTING_PARAMETER_TYPE = SettingParameterType.Graphic;

        SettingParameter[] parameters;

        public void Initialize()
        {
            parameters = new SettingParameter[]
            {
                new SettingParameter(SETTING_PARAMETER_TYPE, PRESET, 0), // Dropdown
                new SettingParameter(SETTING_PARAMETER_TYPE, RESOLUTION, new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height)), // Dropdown
                new SettingParameter(SETTING_PARAMETER_TYPE, DISPLAY_TYPE, Screen.fullScreen ? 0 : 1), // Dropdown
                new SettingParameter(SETTING_PARAMETER_TYPE, VSYNC, false), // Toggle
                new SettingParameter(SETTING_PARAMETER_TYPE, ANTI_ALIAS, QualitySettings.antiAliasing / 2), // Dropdown
                new SettingParameter(SETTING_PARAMETER_TYPE, BRIGHTNESS, Screen.brightness), // Slider 0-1
                new SettingParameter(SETTING_PARAMETER_TYPE, SHADOW_QUALITY, 0), // Dropdown
                new SettingParameter(SETTING_PARAMETER_TYPE, TEXTURE_QUALITY, 0), // Dropdown
            };
        }

        public SettingParameterType GetParameterType() => SETTING_PARAMETER_TYPE;

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
            public int preset;
            public Vector2Int resolution;
            public int displayType;
            public bool vsync;
            public int antiAlias;
            public float brightness;
            public int shadowQuality;
            public int textureQuality;
        }

        object ISavable.GetSaveData()
        {
            return new SaveData
            {
                preset = GetParameter(presetHash).ReadValue<int>(),
                resolution = GetParameter(resolutionHash).ReadValue<Vector2Int>(),
                displayType = GetParameter(displayTypeHash).ReadValue<int>(),
                vsync = GetParameter(vsyncHash).ReadValue<bool>(),
                antiAlias = GetParameter(antiAliasHash).ReadValue<int>(),
                brightness = GetParameter(brightnessHash).ReadValue<float>(),
                shadowQuality = GetParameter(shadowQualityHash).ReadValue<int>(),
                textureQuality = GetParameter(textureQualityHash).ReadValue<int>(),
            };
        }

        void ISavable.LoadSaveData(object data)
        {
            var saveData = (SaveData)data;
            SetParameter(presetHash, saveData.preset);
            SetParameter(resolutionHash, saveData.resolution);
            SetParameter(displayTypeHash, saveData.displayType);
            SetParameter(vsyncHash, saveData.vsync);
            SetParameter(antiAliasHash, saveData.antiAlias);
            SetParameter(brightnessHash, saveData.brightness);
            SetParameter(shadowQualityHash, saveData.shadowQuality);
            SetParameter(textureQualityHash, saveData.textureQuality);
        }
    }
}