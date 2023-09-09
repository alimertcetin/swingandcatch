using UnityEngine;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas;

namespace XIV_Packages.PCSettingSystems.Extras.SettingApplyCommands.GraphicCommands
{
    public class AntialiasingApplyCommand : ApplyCommand<AntiAliasingSetting>
    {
        public override void Apply(AntiAliasingSetting value)
        {
            QualitySettings.antiAliasing = value.antiAliasing;
        }
    }
}