using UnityEngine;
using XIV_Packages.PCSettingSystems.Core;

namespace XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas
{
    [System.Serializable]
    public struct ShadowQualitySetting : ISetting
    {
        public int shadowCascades;
        public float shadowDistance;
        public ShadowmaskMode shadowmaskMode;
        public ShadowResolution shadowResolution;
        public ShadowQuality shadows;

        bool ISetting.canIncludeInPresets => true;

        bool ISetting.IsCritical => false;

        public ShadowQualitySetting(ShadowQualitySetting shadowQualitySetting)
        {
            shadowCascades = shadowQualitySetting.shadowCascades;
            shadowDistance = shadowQualitySetting.shadowDistance;
            shadowmaskMode = shadowQualitySetting.shadowmaskMode;
            shadowResolution = shadowQualitySetting.shadowResolution;
            shadows = shadowQualitySetting.shadows;
        }

        public ShadowQualitySetting(int shadowCascades, float shadowDistance, ShadowmaskMode shadowmaskMode, ShadowResolution shadowResolution, ShadowQuality shadows)
        {
            this.shadowCascades = shadowCascades;
            this.shadowDistance = shadowDistance;
            this.shadowmaskMode = shadowmaskMode;
            this.shadowResolution = shadowResolution;
            this.shadows = shadows;
        }

        //public ShadowQualitySetting GetSetting(SettingQualityLevel settingQualityLevel)
        //{
        //    int shadowCascades;
        //    float shadowDistance;
        //    ShadowmaskMode shadowmaskMode;
        //    ShadowQuality shadows;
        //    ShadowResolution shadowResolution;

        //    switch (settingQualityLevel)
        //    {
        //        case SettingQualityLevel.VeryLow:
        //            shadowCascades = 2;
        //            shadowDistance = 20f;
        //            shadowmaskMode = ShadowmaskMode.Shadowmask;
        //            shadowResolution = ShadowResolution.Low;
        //            shadows = ShadowQuality.HardOnly;
        //            break;
        //        case SettingQualityLevel.Low:
        //            shadowCascades = 4;
        //            shadowDistance = 30f;
        //            shadowmaskMode = ShadowmaskMode.Shadowmask;
        //            shadowResolution = ShadowResolution.Low;
        //            shadows = ShadowQuality.HardOnly;
        //            break;
        //        case SettingQualityLevel.Medium:
        //            shadowCascades = 4;
        //            shadowDistance = 40f;
        //            shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
        //            shadowResolution = ShadowResolution.Medium;
        //            shadows = ShadowQuality.All;
        //            break;
        //        case SettingQualityLevel.High:
        //            shadowCascades = 4;
        //            shadowDistance = 50f;
        //            shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
        //            shadowResolution = ShadowResolution.High;
        //            shadows = ShadowQuality.All;
        //            break;
        //        case SettingQualityLevel.VeryHigh:
        //            shadowCascades = 4;
        //            shadowDistance = 60f;
        //            shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
        //            shadowResolution = ShadowResolution.VeryHigh;
        //            shadows = ShadowQuality.All;
        //            break;
        //        case SettingQualityLevel.Custom:
        //        case SettingQualityLevel.Default:
        //        default:
        //            shadowCascades = 4;
        //            shadowDistance = 50f;
        //            shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
        //            shadowResolution = ShadowResolution.Medium;
        //            shadows = ShadowQuality.All;
        //            break;
        //    }
        //    return new ShadowQualitySetting(
        //        settingQualityLevel,
        //        shadowCascades,
        //        shadowDistance,
        //        shadowmaskMode,
        //        shadowResolution,
        //        shadows);
        //}

        public override string ToString()
        {
            var values = "";
            values += nameof(shadowCascades) + " : " + shadowCascades + ", ";
            values += nameof(shadowDistance) + " : " + shadowDistance + ", ";
            values += nameof(shadowmaskMode) + " : " + shadowmaskMode + ", ";
            values += nameof(shadowResolution) + " : " + shadowResolution + ", ";
            values += nameof(shadows) + " : " + shadows;
            return $"{nameof(ShadowQualitySetting)} : {values}";
        }
    }
}