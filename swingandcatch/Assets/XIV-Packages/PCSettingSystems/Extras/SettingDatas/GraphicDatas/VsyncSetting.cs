using XIV_Packages.PCSettingSystems.Core;

namespace XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas
{
    [System.Serializable]
    public struct VsyncSetting : ISetting
    {
        public bool isOn;

        bool ISetting.canIncludeInPresets => false;

        bool ISetting.IsCritical => false;

        public VsyncSetting(VsyncSetting vsyncSetting)
        {
            isOn = vsyncSetting.isOn;
        }

        public VsyncSetting(bool isOn)
        {
            this.isOn = isOn;
        }

        public override string ToString()
        {
            return $"{nameof(VsyncSetting)}, {nameof(isOn)} : {isOn}";
        }
    }
}