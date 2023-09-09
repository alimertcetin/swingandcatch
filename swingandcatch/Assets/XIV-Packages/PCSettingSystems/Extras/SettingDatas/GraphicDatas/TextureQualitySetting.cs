using XIV_Packages.PCSettingSystems.Core;

namespace XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas
{
    [System.Serializable]
    public struct TextureQualitySetting : ISetting
    {
        /// <summary>
        /// 0 is Very High
        /// </summary>
        public int masterTextureLimit;

        bool ISetting.canIncludeInPresets => true;

        bool ISetting.IsCritical => false;

        public TextureQualitySetting(TextureQualitySetting setting)
        {
            masterTextureLimit = setting.masterTextureLimit;
        }

        public TextureQualitySetting(int masterTextureLimit)
        {
            this.masterTextureLimit = masterTextureLimit;
        }

        public override string ToString()
        {
            return $"{nameof(TextureQualitySetting)}, {nameof(masterTextureLimit)} : {masterTextureLimit}";
        }
    }
}