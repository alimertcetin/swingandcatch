using UnityEngine;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas;

namespace XIV_Packages.PCSettingSystems.Extras.SettingApplyCommands.GraphicCommands
{
    public class VsyncApplyCommand : ApplyCommand<VsyncSetting>
    {
        public override void Apply(VsyncSetting value)
        {
            QualitySettings.vSyncCount = value.isOn ? 1 : 0;
        }
    }
}