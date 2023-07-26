using TheGame.ScriptableObjects.Channels;
using TheGame.SettingSystems;
using TheGame.UISystems.Components;
using TheGame.UISystems.TabSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TheGame.UISystems.MainMenu
{
    public class MainMenuAudioSettingsUI : TabPageUI, ISettingsListener
    {
        [SerializeField] SettingsChannelSO settingsLoaded;
        
        [SerializeField] SettingSlider masterVolumeSlider;
        [SerializeField] SettingSlider musicVolumeSlider;
        [SerializeField] SettingSlider effectVolumeSlider;

        Settings settings;

        void OnEnable()
        {
            settingsLoaded.Register(OnSettingsLoaded);
            masterVolumeSlider.slider.onValueChanged.AddListener(OnMasterVolumeChanged);
            musicVolumeSlider.slider.onValueChanged.AddListener(OnMusicVolumeChanged);
            effectVolumeSlider.slider.onValueChanged.AddListener(OnEffectVolumeChanged);
            this.settings?.AddListener(this);
        }

        void OnDisable()
        {
            settingsLoaded.Unregister(OnSettingsLoaded);
            masterVolumeSlider.slider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            musicVolumeSlider.slider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            effectVolumeSlider.slider.onValueChanged.RemoveListener(OnEffectVolumeChanged);
            this.settings?.RemoveListener(this);
        }

        public override void OnFocus()
        {
            EventSystem.current.SetSelectedGameObject(masterVolumeSlider.GetComponentInChildren<Selectable>().gameObject);
        }

        void OnMasterVolumeChanged(float value01)
        {
            ChangeSetting(AudioSettingsParameterContainer.masterVolumeHash, value01);
        }

        void OnMusicVolumeChanged(float value01)
        {
            ChangeSetting(AudioSettingsParameterContainer.musicVolumeHash, value01);
        }

        void OnEffectVolumeChanged(float value01)
        {
            ChangeSetting(AudioSettingsParameterContainer.effectsVolumeHash, value01);
        }

        void ChangeSetting(int parameterNameHash, float value01)
        {
            settings?.SetParameter(SettingParameterType.Audio, parameterNameHash, value01);
        }

        void OnSettingsLoaded(Settings settings)
        {
            this.settings = settings;
            InitializeUIItems();
            this.settings.AddListener(this);
        }

        void InitializeUIItems()
        {
            var masterVolume = settings.GetParameter(SettingParameterType.Audio, AudioSettingsParameterContainer.masterVolumeHash).ReadValue<float>();
            var musicVolume = settings.GetParameter(SettingParameterType.Audio, AudioSettingsParameterContainer.musicVolumeHash).ReadValue<float>();
            var effectVolume = settings.GetParameter(SettingParameterType.Audio, AudioSettingsParameterContainer.effectsVolumeHash).ReadValue<float>();
            
            masterVolumeSlider.UpdateValue(masterVolume, true);
            musicVolumeSlider.UpdateValue(musicVolume, true);
            effectVolumeSlider.UpdateValue(effectVolume, true);
        }

        void ISettingsListener.OnSettingsChanged(SettingParameter changedParameter)
        {
            var nameHash = changedParameter.nameHash;
            if (nameHash == AudioSettingsParameterContainer.masterVolumeHash)
            {
                masterVolumeSlider.UpdateValue(changedParameter.ReadValue<float>(), true);
            }
            else if (nameHash == AudioSettingsParameterContainer.musicVolumeHash)
            {
                musicVolumeSlider.UpdateValue(changedParameter.ReadValue<float>(), true);
            }
            else if (nameHash == AudioSettingsParameterContainer.effectsVolumeHash)
            {
                effectVolumeSlider.UpdateValue(changedParameter.ReadValue<float>(), true);
            }
        }
    }
}