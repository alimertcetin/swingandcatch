using XIV_Packages.PCSettingSystems.Core;

namespace XIV_Packages.PCSettingSystems.Extras.SettingDatas.AudioDatas
{
    public interface IAudioSetting : ISetting
    {
        /// <summary>
        /// The name of the parameter in <see cref="UnityEngine.Audio.AudioMixer"/>
        /// </summary>
        public string MixerParameter { get; }

        /// <summary>
        /// A number between 0 and 1
        /// </summary>
        public float Value01 { get; }
    }
}