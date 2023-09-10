using XIV_Packages.PCSettingSystems.Core;

namespace XIV_Packages.PCSettingSystems.Extras.SettingDatas.AudioDatas
{
    [System.Serializable]
    public struct MasterAudioSetting : IAudioSetting
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string MixerParameter { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public float Value01 { get; set; }

        bool ISetting.canIncludeInPresets => false;

        bool ISetting.IsCritical => false;

        public MasterAudioSetting(MasterAudioSetting setting)
        {
            MixerParameter = setting.MixerParameter;
            Value01 = setting.Value01;
        }

        public MasterAudioSetting(string mixerParameter, float value01)
        {
            this.MixerParameter = mixerParameter;
            Value01 = value01;
        }

        public override string ToString()
        {
            return $"{nameof(MixerParameter)} : {MixerParameter}, {nameof(Value01)} : {Value01}";
        }
    }
}