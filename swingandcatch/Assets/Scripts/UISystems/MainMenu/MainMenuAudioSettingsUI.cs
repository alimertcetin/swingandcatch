using System;
using System.Collections.Generic;
using TheGame.UISystems.Components;
using TheGame.UISystems.TabSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.ScriptableObjects.Channels;
using XIV_Packages.PCSettingSystems.Extras.SettingContainers;
using XIV_Packages.PCSettingSystems.Extras.SettingDatas.AudioDatas;

namespace TheGame.UISystems.MainMenu
{
    public class MainMenuAudioSettingsUI : TabPageUI, ISettingListener
    {
        [SerializeField] XIVSettingChannelSO settingsLoaded;
        
        [SerializeField] SettingSlider masterVolumeSlider;
        [SerializeField] SettingSlider musicVolumeSlider;
        [SerializeField] SettingSlider effectVolumeSlider;

        AudioSettingContainer audioSettingContainer;
        Dictionary<Type, Action<IAudioSetting>> onSettingChangeLookup = new Dictionary<Type, Action<IAudioSetting>>();

        protected override void Awake()
        {
            base.Awake();
            onSettingChangeLookup.Add(typeof(MasterAudioSetting), UpdateMasterAudioSlider);
            onSettingChangeLookup.Add(typeof(MusicAudioSetting), UpdateMusicAudioSlider);
            onSettingChangeLookup.Add(typeof(EffectAudioSetting), UpdateEffectAudioSlider);
        }

        void OnEnable()
        {
            settingsLoaded.Register(OnSettingsLoaded);
            masterVolumeSlider.slider.onValueChanged.AddListener(OnMasterVolumeChanged);
            musicVolumeSlider.slider.onValueChanged.AddListener(OnMusicVolumeChanged);
            effectVolumeSlider.slider.onValueChanged.AddListener(OnEffectVolumeChanged);
            this.audioSettingContainer?.AddListener(this);
        }

        void OnDisable()
        {
            settingsLoaded.Unregister(OnSettingsLoaded);
            masterVolumeSlider.slider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            musicVolumeSlider.slider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            effectVolumeSlider.slider.onValueChanged.RemoveListener(OnEffectVolumeChanged);
            this.audioSettingContainer?.RemoveListener(this);
        }

        public override void OnFocus()
        {
            EventSystem.current.SetSelectedGameObject(masterVolumeSlider.GetComponentInChildren<Selectable>().gameObject);
        }

        // Update visuals when settings changed

        void UpdateMasterAudioSlider(IAudioSetting setting)
        {
            masterVolumeSlider.UpdateValue(setting.Value01, false);
        }

        void UpdateMusicAudioSlider(IAudioSetting setting)
        {
            musicVolumeSlider.UpdateValue(setting.Value01, false);
        }

        void UpdateEffectAudioSlider(IAudioSetting setting)
        {
            effectVolumeSlider.UpdateValue(setting.Value01, false);
        }

        // Direct interaction with GraphicSettingContainer - Change settings on value change

        void OnMasterVolumeChanged(float value01)
        {
            ChangeSetting(new MasterAudioSetting(AudioMixerConstants.DefaultMixer.Parameters.MasterVolume, value01));
        }

        void OnMusicVolumeChanged(float value01)
        {
            ChangeSetting(new MusicAudioSetting(AudioMixerConstants.DefaultMixer.Parameters.MusicVolume, value01));
        }

        void OnEffectVolumeChanged(float value01)
        {
            ChangeSetting(new EffectAudioSetting(AudioMixerConstants.DefaultMixer.Parameters.EffectsVolume, value01));
        }

        void ChangeSetting<T>(T setting) where T : struct, IAudioSetting
        {
            audioSettingContainer.ChangeSetting(setting);
            audioSettingContainer.ApplyChanges();
            audioSettingContainer.ClearUndoHistory();
        }

        void OnSettingsLoaded(XIVSettings settings)
        {
            audioSettingContainer = settings.GetContainer<AudioSettingContainer>();
            InitializeUIItems();
            audioSettingContainer.AddListener(this);
        }

        void InitializeUIItems()
        {
            var masterVolume = audioSettingContainer.GetSetting<MasterAudioSetting>().Value01;
            var musicVolume = audioSettingContainer.GetSetting<MusicAudioSetting>().Value01;
            var effectVolume = audioSettingContainer.GetSetting<EffectAudioSetting>().Value01;

            masterVolumeSlider.UpdateValue(masterVolume, true);
            musicVolumeSlider.UpdateValue(musicVolume, true);
            effectVolumeSlider.UpdateValue(effectVolume, true);
        }

        void ISettingListener.OnSettingChanged(SettingChange settingChange)
        {
            onSettingChangeLookup.TryGetValue(settingChange.settingType, out var value);
            value.Invoke((IAudioSetting)settingChange.to);
        }

        void ISettingListener.OnBeforeApply(ISettingContainer _) { }

        void ISettingListener.OnAfterApply(ISettingContainer _) { }
    }
}