using XIV_Packages.PCSettingSystems.Core;

namespace XIV_Packages.PCSettingSystems.Extras.SettingDatas.AudioDatas
{
    [System.Serializable]
    public readonly struct MusicAudioSetting : IAudioSetting
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public readonly string MixerParameter { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public readonly float Value01 { get; }

        bool ISetting.canIncludeInPresets => false;

        bool ISetting.IsCritical => false;

        public MusicAudioSetting(MusicAudioSetting setting)
        {
            MixerParameter = setting.MixerParameter;
            Value01 = setting.Value01;
        }

        public MusicAudioSetting(string mixerParameter, float value01)
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