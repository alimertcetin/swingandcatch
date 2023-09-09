using UnityEngine;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas;

namespace XIV_Packages.PCSettingSystems.Extras.SettingApplyCommands.GraphicCommands
{
    public class TextureQualityApplyCommand : ApplyCommand<TextureQualitySetting>
    {
        public override void Apply(TextureQualitySetting value)
        {
            QualitySettings.masterTextureLimit = value.masterTextureLimit;
        }
    }
}