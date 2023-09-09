using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.SettingAppliers;
using XIV_Packages.PCSettingSystems.Extras.SettingApplyCommands.AudioCommands;
using XIV_Packages.PCSettingSystems.Extras.SettingContainers;
using XIV_Packages.PCSettingSystems.Extras.SettingDatas.AudioDatas;

namespace XIV_Packages.PCSettingSystems.Extras
{
    public class AudioSettingManager : SettingManager
    {
        [SerializeField] AudioMixer mixer;

        AudioSettingContainer audioSettingContainer;

        public override void InitializeContainer()
        {
            audioSettingContainer = new AudioSettingContainer(CreateAudioSettingApplier());
            var audioSettings = new List<ISetting>
            {
                new MasterAudioSetting(AudioMixerConstants.DefaultMixer.Parameters.MasterVolume, 0.75f),
                new MusicAudioSetting(AudioMixerConstants.DefaultMixer.Parameters.MusicVolume, 0.5f),
                new EffectAudioSetting(AudioMixerConstants.DefaultMixer.Parameters.EffectsVolume, 0.5f),
            };
            audioSettingContainer.InitializeSettings(audioSettings);
            audioSettingContainer.ApplyChanges();
            audioSettingContainer.ClearUndoHistory();
        }

        public override ISettingContainer GetContainer() => audioSettingContainer;

        ISettingApplier CreateAudioSettingApplier()
        {
            ISettingApplier settingApplier = new AudioSettingApplier();
            settingApplier.AddApplyCommand(new AudioSettingApplyCommand(mixer));
            return settingApplier;
        }
    }
}