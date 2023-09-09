using UnityEngine;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas;

namespace XIV_Packages.PCSettingSystems.Extras.SettingApplyCommands.GraphicCommands
{
    public class ShadowQualityApplyCommand : ApplyCommand<ShadowQualitySetting>
    {
        public override void Apply(ShadowQualitySetting value)
        {
            QualitySettings.shadowCascades = value.shadowCascades;
            QualitySettings.shadowDistance = value.shadowDistance;
            QualitySettings.shadowmaskMode = value.shadowmaskMode;
            QualitySettings.shadowResolution = value.shadowResolution;
            QualitySettings.shadows = value.shadows;
        }
    }
}