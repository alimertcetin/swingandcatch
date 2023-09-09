using XIV_Packages.PCSettingSystems.Core;

namespace XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas
{
    [System.Serializable]
    public struct DisplayTypeSetting : ISetting
    {
        public bool isFullScreen;

        bool ISetting.canIncludeInPresets => false;

        bool ISetting.IsCritical => true;

        public DisplayTypeSetting(DisplayTypeSetting displayTypeSetting)
        {
            isFullScreen = displayTypeSetting.isFullScreen;
        }

        public DisplayTypeSetting(bool isFullScreen)
        {
            this.isFullScreen = isFullScreen;
        }

        public override string ToString()
        {
            return $"{nameof(DisplayTypeSetting)}, {nameof(isFullScreen)} : {isFullScreen}";
        }
    }
}