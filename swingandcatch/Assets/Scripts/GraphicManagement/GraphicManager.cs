using TheGame.AudioManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.SettingSystems;
using UnityEngine;

namespace TheGame.GraphicManagement
{
    public class GraphicManager : MonoBehaviour, ISettingsListener
    {
        [SerializeField] SettingsChannelSO settingsLoaded;
        [SerializeField] GraphicSettingPresetSO veryHighPreset;
        [SerializeField] GraphicSettingPresetSO highPreset;
        [SerializeField] GraphicSettingPresetSO mediumPreset;
        [SerializeField] GraphicSettingPresetSO lowPreset;
        [SerializeField] GraphicSettingPresetSO veryLow;
        
        Settings settings;
        int currentPreset;
        bool presetLock;

        void Awake()
        {
            Application.targetFrameRate = 60;
        }
        
        void OnEnable()
        {
            settingsLoaded.Register(OnSettingsLoaded);
            this.settings?.AddListener(this);
        }

        void OnDisable()
        {
            settingsLoaded.Unregister(OnSettingsLoaded);
            this.settings?.RemoveListener(this);
        }

        void OnSettingsLoaded(Settings settings)
        {
            this.settings = settings;
            var presetParameter = settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.presetHash);
            currentPreset = presetParameter.ReadValue<int>();
            
            UpdateSettings(presetParameter);
            UpdateSettings(settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.resolutionHash));
            UpdateSettings(settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.displayTypeHash));
            UpdateSettings(settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.vsyncHash));
            UpdateSettings(settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.antiAliasHash));
            UpdateSettings(settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.brightnessHash));
            UpdateSettings(settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.shadowQualityHash));
            UpdateSettings(settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.textureQualityHash));
            
            this.settings.AddListener(this);
        }

        void ISettingsListener.OnSettingsChanged(SettingParameter changedParameter)
        {
            // TODO : GraphicManager -> Apply Command pattern
            if (changedParameter.settingParameterType != SettingParameterType.Graphic) return;

            UpdateSettings(changedParameter);
        }

        void UpdateSettings(SettingParameter changedParameter)
        {
            if (presetLock) return;
            
            var nameHash = changedParameter.nameHash;
            if (GraphicSettingsParameterContainer.presetHash == nameHash)
            {
                presetLock = true;
                
                void SetParameter(int hash, object value)
                {
                    settings.SetParameter(SettingParameterType.Graphic, hash, value);
                }

                var preset = changedParameter.ReadValue<int>();
                if (preset < 0)
                {
                    presetLock = false;
                    return;
                }

                var graphicPreset = GetPreset(preset);
                
                SetParameter(GraphicSettingsParameterContainer.antiAliasHash, graphicPreset.antiAliasing);
                SetAntiAliasing(new SettingParameter { value = graphicPreset.antiAliasing });
                SetParameter(GraphicSettingsParameterContainer.shadowQualityHash, graphicPreset.shadowQuality);
                SetShadowQuality(new SettingParameter { value = graphicPreset.shadowQuality });
                SetParameter(GraphicSettingsParameterContainer.textureQualityHash, graphicPreset.textureQuality);
                SetTextureQuality(new SettingParameter { value = graphicPreset.textureQuality });

                presetLock = false;
                currentPreset = preset;
                return;
            }
            else if (GraphicSettingsParameterContainer.resolutionHash == nameHash)
            {
                SetResolution(changedParameter);
            }
            else if (GraphicSettingsParameterContainer.displayTypeHash == nameHash)
            {
                SetDisplayType(changedParameter);
            }
            else if (GraphicSettingsParameterContainer.vsyncHash == nameHash)
            {
                SetVsync(changedParameter);
            }
            else if (GraphicSettingsParameterContainer.antiAliasHash == nameHash)
            {
                SetAntiAliasing(changedParameter);
            }
            else if (GraphicSettingsParameterContainer.brightnessHash == nameHash)
            {
                SetBrightness(changedParameter);
            }
            else if (GraphicSettingsParameterContainer.shadowQualityHash == nameHash)
            {
                SetShadowQuality(changedParameter);
            }
            else if (GraphicSettingsParameterContainer.textureQualityHash == nameHash)
            {
                SetTextureQuality(changedParameter);
            }

            int newPreset = GetCurrentPreset();
            if (newPreset != currentPreset)
            {
                settings.SetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.presetHash, newPreset);
            }
        }

        static void SetVsync(SettingParameter changedParameter)
        {
            QualitySettings.vSyncCount = changedParameter.ReadValue<bool>() ? 1 : 0;
        }

        static void SetAntiAliasing(SettingParameter changedParameter)
        {
            QualitySettings.antiAliasing = changedParameter.ReadValue<int>() * 2;
        }

        static void SetBrightness(SettingParameter changedParameter)
        {
            Screen.brightness = changedParameter.ReadValue<float>();
        }

        static void SetTextureQuality(SettingParameter changedParameter)
        {
            // 0 full resolution, 1 half resolution, 2 quarter resolution
            // 2 - inverts the value
            QualitySettings.masterTextureLimit = 2 - changedParameter.ReadValue<int>();
        }

        void SetResolution(SettingParameter parameter)
        {
            var resolution = parameter.ReadValue<Vector2Int>();
            var displayType = settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.displayTypeHash).ReadValue<int>();
            Screen.SetResolution(resolution.x, resolution.y, displayType == 0);
        }

        static void SetDisplayType(SettingParameter parameter)
        {
            Screen.fullScreen = parameter.ReadValue<int>() == 0;
        }

        void SetShadowQuality(SettingParameter parameter)
        {
            int shadowQuality = parameter.ReadValue<int>();
            void SetShadowSettings(ShadowmaskMode shadowmaskMode, ShadowQuality shadowQuality, ShadowResolution shadowResolution, ShadowProjection shadowProjection, 
                float shadowDistance, float shadowNearPlaneOffset, int shadowCascades)
            {
                QualitySettings.shadowmaskMode = shadowmaskMode;
                QualitySettings.shadows = shadowQuality;
                QualitySettings.shadowResolution = shadowResolution;
                QualitySettings.shadowProjection = shadowProjection;
                QualitySettings.shadowDistance = shadowDistance;
                QualitySettings.shadowNearPlaneOffset = shadowNearPlaneOffset;
                QualitySettings.shadowCascades = shadowCascades;
            }
            
            switch (shadowQuality)
            {
                case 0:
                    SetShadowSettings(ShadowmaskMode.DistanceShadowmask, ShadowQuality.All, ShadowResolution.VeryHigh, ShadowProjection.StableFit, 150f, 3f, 4);
                    break;
                case 1:
                    SetShadowSettings(ShadowmaskMode.DistanceShadowmask, ShadowQuality.All, ShadowResolution.High, ShadowProjection.StableFit, 70f, 3f, 4);
                    break;
                case 2:
                    SetShadowSettings(ShadowmaskMode.DistanceShadowmask, ShadowQuality.All, ShadowResolution.Medium, ShadowProjection.StableFit, 40f, 3f, 2);
                    break;
                case 3:
                    SetShadowSettings(ShadowmaskMode.Shadowmask, ShadowQuality.HardOnly, ShadowResolution.Low, ShadowProjection.StableFit, 20f, 3f, 0);
                    break;
                case 4:
                    SetShadowSettings(ShadowmaskMode.Shadowmask, ShadowQuality.Disable, ShadowResolution.Low, ShadowProjection.StableFit, 15f, 3f, 0);
                    break;
            }
        }

        int GetCurrentPreset()
        {
            bool IsMatching(GraphicSettingPresetSO graphicSettingPresetSO, bool vsync, int antiAliasing, int shadowQuality, int textureQuality)
            {
                return graphicSettingPresetSO.antiAliasing == antiAliasing && 
                       graphicSettingPresetSO.shadowQuality == shadowQuality && 
                       graphicSettingPresetSO.textureQuality == textureQuality;
            }

            var vsync = settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.vsyncHash).ReadValue<bool>();
            var antiAliasing = settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.antiAliasHash).ReadValue<int>();
            var shadowQuality = settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.shadowQualityHash).ReadValue<int>();
            var textureQuality = settings.GetParameter(SettingParameterType.Graphic, GraphicSettingsParameterContainer.textureQualityHash).ReadValue<int>();

            if (IsMatching(veryHighPreset, vsync, antiAliasing, shadowQuality, textureQuality))
            {
                return 0;
            }

            if (IsMatching(highPreset, vsync, antiAliasing, shadowQuality, textureQuality))
            {
                return 1;
            }

            if (IsMatching(mediumPreset, vsync, antiAliasing, shadowQuality, textureQuality))
            {
                return 2;
            }
            
            if (IsMatching(lowPreset, vsync, antiAliasing, shadowQuality, textureQuality))
            {
                return 3;
            }
            
            if (IsMatching(veryLow, vsync, antiAliasing, shadowQuality, textureQuality))
            {
                return 4;
            }

            return -1;
        }
        
        GraphicSettingPresetSO GetPreset(int presetValue)
        {
            return presetValue switch
            {
                0 => veryHighPreset,
                1 => highPreset,
                2 => mediumPreset,
                3 => lowPreset,
                4 => veryLow,
                _ => default,
            };
        }
    }
}