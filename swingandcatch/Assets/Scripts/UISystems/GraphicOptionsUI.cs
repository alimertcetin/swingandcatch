using System.Collections.Generic;
using TheGame.ScriptableObjects.Channels;
using TheGame.SettingSystems;
using TheGame.UISystems.Components;
using TheGame.UISystems.Core;
using TMPro;
using UnityEngine;

namespace TheGame.UISystems
{
    public class GraphicOptionsUI : GameUI, ISettingsListener
    {
        [SerializeField] SettingsChannelSO settingsLoaded;
        
        [Header("UI Elements for Graphic Settings")]
        [SerializeField] SettingDropDrown presetDropdown;
        [SerializeField] SettingDropDrown resolutionDropdown;
        [SerializeField] SettingDropDrown displayTypeDropdown;
        [SerializeField] SettingToggle vsyncToggle;
        [SerializeField] SettingDropDrown antiAliasDropdown;
        [SerializeField] SettingSlider brightnessSlider;
        [SerializeField] SettingDropDrown shadowQualityDropdown;
        [SerializeField] SettingDropDrown textureQualityDropdown;

        Settings settings;

        void OnEnable()
        {
            settingsLoaded.Register(OnSettingsLoaded);
            presetDropdown.dropDown.onValueChanged.AddListener(OnPresetValueChanged);
            resolutionDropdown.dropDown.onValueChanged.AddListener(OnResolutionValueChanged);
            displayTypeDropdown.dropDown.onValueChanged.AddListener(OnDisplayTypeValueChanged);
            vsyncToggle.toggle.onValueChanged.AddListener(OnVsyncValueChanged);
            antiAliasDropdown.dropDown.onValueChanged.AddListener(OnAntiAliasValuChanged);
            brightnessSlider.slider.onValueChanged.AddListener(OnBrightnessValueChanged);
            shadowQualityDropdown.dropDown.onValueChanged.AddListener(OnShadowQualityValueChanged);
            textureQualityDropdown.dropDown.onValueChanged.AddListener(OnTextureQualityValueChanged);
            this.settings?.AddListener(this);
        }

        void OnDisable()
        {
            settingsLoaded.Unregister(OnSettingsLoaded);
            presetDropdown.dropDown.onValueChanged.RemoveListener(OnPresetValueChanged);
            resolutionDropdown.dropDown.onValueChanged.RemoveListener(OnResolutionValueChanged);
            displayTypeDropdown.dropDown.onValueChanged.RemoveListener(OnDisplayTypeValueChanged);
            vsyncToggle.toggle.onValueChanged.RemoveListener(OnVsyncValueChanged);
            antiAliasDropdown.dropDown.onValueChanged.RemoveListener(OnAntiAliasValuChanged);
            brightnessSlider.slider.onValueChanged.RemoveListener(OnBrightnessValueChanged);
            shadowQualityDropdown.dropDown.onValueChanged.RemoveListener(OnShadowQualityValueChanged);
            textureQualityDropdown.dropDown.onValueChanged.RemoveListener(OnTextureQualityValueChanged);
            this.settings?.RemoveListener(this);
        }

        void OnPresetValueChanged(int value)
        {
            if (value == presetDropdown.dropDown.options.Count - 1) value = -1; // Last one is the custom one
            SetParameter(GraphicSettingsParameterContainer.presetHash, value);
        }

        void OnResolutionValueChanged(int value)
        {
            SetParameter(GraphicSettingsParameterContainer.resolutionHash, GetValueOfIndex(resolutionDropdown, value));
        }

        void OnDisplayTypeValueChanged(int value)
        {
            SetParameter(GraphicSettingsParameterContainer.displayTypeHash, value);
        }

        void OnVsyncValueChanged(bool value)
        {
            SetParameter(GraphicSettingsParameterContainer.vsyncHash, value);
        }

        void OnAntiAliasValuChanged(int value)
        {
            SetParameter(GraphicSettingsParameterContainer.antiAliasHash, value);
        }

        void OnBrightnessValueChanged(float value)
        {
            SetParameter(GraphicSettingsParameterContainer.brightnessHash, value);
        }

        void OnShadowQualityValueChanged(int value)
        {
            SetParameter(GraphicSettingsParameterContainer.shadowQualityHash, value);
        }

        void OnTextureQualityValueChanged(int value)
        {
            SetParameter(GraphicSettingsParameterContainer.textureQualityHash, value);
        }

        void SetParameter(int nameHash, object value)
        {
            settings?.SetParameter(SettingParameterType.Graphic, nameHash, value);
        }

        void OnSettingsLoaded(Settings settings)
        {
            this.settings = settings;
            InitializeUIItems();
            this.settings.AddListener(this);
        }

        void InitializeUIItems()
        {
            T GetParameterValue<T>(int hash) => settings.GetParameter(SettingParameterType.Graphic, hash).ReadValue<T>();

            resolutionDropdown.dropDown.ClearOptions();
            var resolutionOptions = new List<TMP_Dropdown.OptionData>();
            var resolutions = Screen.resolutions;
            int length = resolutions.Length;
            for (int i = 0; i < length; i++)
            {
                var res = resolutions[i];
                var resText = res.width + "x" + res.height;
                resolutionOptions.Add(new TMP_Dropdown.OptionData(resText));
            }
            resolutionDropdown.dropDown.AddOptions(resolutionOptions);

            var presetValue = GetParameterValue<int>(GraphicSettingsParameterContainer.presetHash);
            presetValue = presetValue < 0 ? 5 : presetValue;
            presetDropdown.UpdateValue(presetValue, true);
            
            int indexOfResolution = GetIndexOfValue(resolutionDropdown, GetParameterValue<Vector2Int>(GraphicSettingsParameterContainer.resolutionHash));
            indexOfResolution = indexOfResolution < 0 ? 0 : indexOfResolution;
            resolutionDropdown.UpdateValue(indexOfResolution, true);
            
            displayTypeDropdown.UpdateValue(GetParameterValue<int>(GraphicSettingsParameterContainer.displayTypeHash), true);
            vsyncToggle.UpdateValue(GetParameterValue<bool>(GraphicSettingsParameterContainer.vsyncHash), true);
            antiAliasDropdown.UpdateValue(GetParameterValue<int>(GraphicSettingsParameterContainer.antiAliasHash), true);
            brightnessSlider.UpdateValue(GetParameterValue<float>(GraphicSettingsParameterContainer.brightnessHash), true);
            shadowQualityDropdown.UpdateValue(GetParameterValue<int>(GraphicSettingsParameterContainer.shadowQualityHash), true);
            textureQualityDropdown.UpdateValue(GetParameterValue<int>(GraphicSettingsParameterContainer.textureQualityHash), true);
        }

        int GetIndexOfValue(SettingDropDrown dropdown, Vector2Int value)
        {
            var options = dropdown.dropDown.options;
            int count = options.Count;
            for (int i = 0; i < count; i++)
            {
                TMP_Dropdown.OptionData option = options[i];
                var optionSplitted = option.text.ToLower().Split('x');
                int length = optionSplitted.Length;
                if (length < 2) continue;
                if (int.TryParse(optionSplitted[0], out var x) == false) continue;
                if (int.TryParse(optionSplitted[1], out var y) == false) continue;
                if (value.x == x && value.y == y) return i;
            }

            return -1;
        }

        Vector2Int GetValueOfIndex(SettingDropDrown dropdown, int index)
        {
            var options = dropdown.dropDown.options;
            TMP_Dropdown.OptionData option = options[index];
            var optionSplitted = option.text.ToLower().Split('x');
            int length = optionSplitted.Length;
            var current = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
            if (length < 2) return current;
            if (int.TryParse(optionSplitted[0], out var x) == false) return current;
            if (int.TryParse(optionSplitted[1], out var y) == false) return current;

            return new Vector2Int(x, y);
        }
        
        void ISettingsListener.OnSettingsChanged(SettingParameter changedParameter)
        {
            var nameHash = changedParameter.nameHash;
            if (nameHash == GraphicSettingsParameterContainer.presetHash)
            {
                int presetVal = changedParameter.ReadValue<int>();
                presetVal = presetVal < 0 ? presetDropdown.dropDown.options.Count - 1 : presetVal;
                presetDropdown.UpdateValue(presetVal, true);
            }
            else if (nameHash == GraphicSettingsParameterContainer.presetHash)
            {
                resolutionDropdown.UpdateValue(GetIndexOfValue(resolutionDropdown, changedParameter.ReadValue<Vector2Int>()), true);
            }
            else if (nameHash == GraphicSettingsParameterContainer.displayTypeHash)
            {
                displayTypeDropdown.UpdateValue(changedParameter.ReadValue<int>(), true);
            }
            else if (nameHash == GraphicSettingsParameterContainer.vsyncHash)
            {
                vsyncToggle.UpdateValue(changedParameter.ReadValue<bool>(), true);
            }
            else if (nameHash == GraphicSettingsParameterContainer.antiAliasHash)
            {
                antiAliasDropdown.UpdateValue(changedParameter.ReadValue<int>(), true);
            }
            else if (nameHash == GraphicSettingsParameterContainer.brightnessHash)
            {
                brightnessSlider.UpdateValue(changedParameter.ReadValue<float>(), true);
            }
            else if (nameHash == GraphicSettingsParameterContainer.shadowQualityHash)
            {
                shadowQualityDropdown.UpdateValue(changedParameter.ReadValue<int>(), true);
            }
            else if (nameHash == GraphicSettingsParameterContainer.textureQualityHash)
            {
                textureQualityDropdown.UpdateValue(changedParameter.ReadValue<int>(), true);
            }
        }
    }
}