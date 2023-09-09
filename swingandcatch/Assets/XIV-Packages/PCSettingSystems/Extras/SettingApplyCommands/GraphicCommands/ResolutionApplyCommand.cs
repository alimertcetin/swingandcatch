using UnityEngine;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas;

namespace XIV_Packages.PCSettingSystems.Extras.SettingApplyCommands.GraphicCommands
{
    public class ResolutionApplyCommand : ApplyCommand<ResolutionSetting>
    {
        public override void Apply(ResolutionSetting value)
        {
            Screen.SetResolution(value.x, value.y, Screen.fullScreen);
        }
    }
}