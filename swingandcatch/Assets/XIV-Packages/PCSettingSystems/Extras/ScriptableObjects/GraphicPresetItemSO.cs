using TheGame.ScriptableObjects;
using UnityEngine;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas;

namespace XIV_Packages.PCSettingSystems.Extras.ScriptableObjects
{
    [CreateAssetMenu(menuName = MenuPaths.GRAPHIC_BASE_MENU + nameof(GraphicPresetItemSO))]
    public class GraphicPresetItemSO : ScriptableObject
    {
        public SettingQualityLevel settingQualityLevel;
        [Tooltip("antiAliasing = Valid values are 0 (no MSAA), 2, 4, and 8 - settingQualityLevel = how this setting should be defined")]
        public AntiAliasingSetting antiAliasingSetting;
        [Tooltip("settingQualityLevel = how this setting should be defined")]
        public ShadowQualitySetting shadowQualitySetting;
        [Tooltip("masterTextureLimit = 0 is Very High - settingQualityLevel = how this setting should be defined")]
        public TextureQualitySetting textureQualitySetting;

        public SettingPreset GetGraphicSetting()
        {
            var currentResolution = Screen.currentResolution;
            var resolution = new ResolutionSetting(currentResolution.width, currentResolution.height);
            var displayType = new DisplayTypeSetting(Screen.fullScreen);
            var vsynSetting = new VsyncSetting(QualitySettings.vSyncCount == 0);
            return new SettingPreset(settingQualityLevel, new ISetting[]
            {
                antiAliasingSetting, shadowQualitySetting,
                textureQualitySetting, resolution, displayType, vsynSetting,
            });
        }
    }
}