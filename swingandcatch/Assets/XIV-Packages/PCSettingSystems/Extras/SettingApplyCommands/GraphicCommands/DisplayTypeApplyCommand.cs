using UnityEngine;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas;

namespace XIV_Packages.PCSettingSystems.Extras.SettingApplyCommands.GraphicCommands
{
    public class DisplayTypeApplyCommand : ApplyCommand<DisplayTypeSetting>
    {
        public override void Apply(DisplayTypeSetting value)
        {
            Screen.fullScreen = value.isFullScreen;
        }
    }
}