using UnityEngine;
using UnityEngine.Audio;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.SettingDatas.AudioDatas;

namespace XIV_Packages.PCSettingSystems.Extras.SettingApplyCommands.AudioCommands
{
    public class AudioSettingApplyCommand : ApplyCommand<IAudioSetting>
    {
        readonly AudioMixer mixer;

        public AudioSettingApplyCommand(AudioMixer mixer)
        {
            this.mixer = mixer;
        }

        public override void Apply(IAudioSetting value)
        {
            var val = Mathf.Max(value.Value01, Mathf.Epsilon);
            mixer.SetFloat(value.MixerParameter, Mathf.Log10(val) * 20f);
        }
    }
}