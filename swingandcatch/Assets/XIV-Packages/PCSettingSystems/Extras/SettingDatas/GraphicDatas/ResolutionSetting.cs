using XIV_Packages.PCSettingSystems.Core;

namespace XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas
{
    [System.Serializable]
    public struct ResolutionSetting : ISetting
    {
        public int x;
        public int y;

        bool ISetting.canIncludeInPresets => false;

        bool ISetting.IsCritical => true;

        public ResolutionSetting(ResolutionSetting resolutionSetting)
        {
            x = resolutionSetting.x;
            y = resolutionSetting.y;
        }

        public ResolutionSetting(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"{nameof(ResolutionSetting)} : {x}x{y}";
        }
    }
}