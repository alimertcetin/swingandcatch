using System;
using System.Collections.Generic;
using TheGame.UISystems.Components;
using TheGame.UISystems.TabSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;
using XIV.EventSystem;
using XIV.EventSystem.Events;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.ScriptableObjects.Channels;
using XIV_Packages.PCSettingSystems.Extras.SettingContainers;
using XIV_Packages.PCSettingSystems.Extras.SettingDatas.GraphicDatas;

namespace TheGame.UISystems.MainMenu
{
    public class GraphicSettingsTabPageUI : TabPageUI, ISettingListener
    {
        [SerializeField] XIVSettingChannelSO settingsLoaded;
        
        [Header("UI Elements for Graphic Settings")]
        [SerializeField] XIVSettingDropdown presetDropdown;
        [SerializeField] XIVSettingDropdown resolutionDropdown;
        [SerializeField] XIVSettingDropdown displayTypeDropdown;
        [SerializeField] SettingToggle vsyncToggle;
        [SerializeField] XIVSettingDropdown antiAliasDropdown;
        [SerializeField] XIVSettingDropdown shadowQualityDropdown;
        [SerializeField] XIVSettingDropdown textureQualityDropdown;
        [SerializeField] Button btn_ApplyChanges;
        [SerializeField] Button btn_RevertChanges;
        [SerializeField] Button btn_Undo;
        [SerializeField] Button btn_Redo;

        GraphicSettingContainer graphicSettingContainer;
        Dictionary<Type, Action<ISetting>> onSettingChangeLookup = new Dictionary<Type, Action<ISetting>>();

        IEvent revertChangesEvent;

        static readonly string[] qualityOptionStrings = new string[]
        {
                "Very Low", "Low", "Medium", "High", "Very High",
        };

        protected override void Awake()
        {
            base.Awake();
            onSettingChangeLookup.Add(typeof(SettingPreset), UpdatePresetDropdown);
            onSettingChangeLookup.Add(typeof(VsyncSetting), UpdateVsyncToggle);
            onSettingChangeLookup.Add(typeof(AntiAliasingSetting), UpdateAntiAliasingDropdown);
            onSettingChangeLookup.Add(typeof(ShadowQualitySetting), UpdateShadowQualityDropdown);
            onSettingChangeLookup.Add(typeof(TextureQualitySetting), UpdateTextureQualityDropdown);
            onSettingChangeLookup.Add(typeof(ResolutionSetting), UpdateResolutionDropdown);
            onSettingChangeLookup.Add(typeof(DisplayTypeSetting), UpdateDisplayTypeDropdown);
        }

        void OnEnable()
        {
            settingsLoaded.Register(OnSettingsLoaded);
            presetDropdown.dropdown.onValueChanged.AddListener(OnPresetValueChanged);
            resolutionDropdown.dropdown.onValueChanged.AddListener(OnResolutionValueChanged);
            displayTypeDropdown.dropdown.onValueChanged.AddListener(OnDisplayTypeValueChanged);
            vsyncToggle.toggle.onValueChanged.AddListener(OnVsyncValueChanged);
            antiAliasDropdown.dropdown.onValueChanged.AddListener(OnAntiAliasValueChanged);
            shadowQualityDropdown.dropdown.onValueChanged.AddListener(OnShadowQualityValueChanged);
            textureQualityDropdown.dropdown.onValueChanged.AddListener(OnTextureQualityValueChanged);

            btn_ApplyChanges.onClick.AddListener(OnApplyClicked);
            btn_RevertChanges.onClick.AddListener(OnRevertClicked);
            btn_Undo.onClick.AddListener(OnUndoClicked);
            btn_Redo.onClick.AddListener(OnRedoClicked);

            this.graphicSettingContainer?.AddListener(this);
        }

        void OnDisable()
        {
            settingsLoaded.Unregister(OnSettingsLoaded);
            presetDropdown.dropdown.onValueChanged.RemoveListener(OnPresetValueChanged);
            resolutionDropdown.dropdown.onValueChanged.RemoveListener(OnResolutionValueChanged);
            displayTypeDropdown.dropdown.onValueChanged.RemoveListener(OnDisplayTypeValueChanged);
            vsyncToggle.toggle.onValueChanged.RemoveListener(OnVsyncValueChanged);
            antiAliasDropdown.dropdown.onValueChanged.RemoveListener(OnAntiAliasValueChanged);
            shadowQualityDropdown.dropdown.onValueChanged.RemoveListener(OnShadowQualityValueChanged);
            textureQualityDropdown.dropdown.onValueChanged.RemoveListener(OnTextureQualityValueChanged);

            btn_ApplyChanges.onClick.RemoveListener(OnApplyClicked);
            btn_RevertChanges.onClick.RemoveListener(OnRevertClicked);
            btn_Undo.onClick.RemoveListener(OnUndoClicked);
            btn_Redo.onClick.RemoveListener(OnRedoClicked);

            this.graphicSettingContainer?.RemoveListener(this);
        }

        public override void OnFocus()
        {
            EventSystem.current.SetSelectedGameObject(presetDropdown.dropdown.GetComponentInChildren<Selectable>().gameObject);
        }

        void SetInteractableSettingElements(bool val)
        {
            presetDropdown.dropdown.interactable = val;
            resolutionDropdown.dropdown.interactable = val;
            displayTypeDropdown.dropdown.interactable = val;
            vsyncToggle.toggle.interactable = val;
            antiAliasDropdown.dropdown.interactable = val;
            shadowQualityDropdown.dropdown.interactable = val;
            textureQualityDropdown.dropdown.interactable = val;
        }

        void OnApplyClicked()
        {
            btn_ApplyChanges.interactable = false;
            btn_RevertChanges.interactable = true;
            SetInteractableSettingElements(false);

            graphicSettingContainer.ApplyChanges();
            // TODO : Display a dialog box. If ok pressed do nothing. Otherwise revert changes. If nothing clicked for x amount of seconds revert changes.
            revertChangesEvent = new InvokeAfterEvent(10f).OnCompleted(() => 
            {
                btn_ApplyChanges.interactable = true;
                btn_RevertChanges.interactable = false;
                graphicSettingContainer.Undo();
                SetInteractableSettingElements(true);
            });
            XIVEventSystem.SendEvent(revertChangesEvent);
        }

        void OnRevertClicked()
        {
            btn_ApplyChanges.interactable = true;
            btn_RevertChanges.interactable = false;
            XIVEventSystem.CancelEvent(revertChangesEvent);
            graphicSettingContainer.Undo();
            SetInteractableSettingElements(true);
        }

        void OnUndoClicked()
        {
            graphicSettingContainer.Undo();
        }

        void OnRedoClicked()
        {
            graphicSettingContainer.Redo();
        }

        // Update visuals when settings changed

        void UpdatePresetDropdown(ISetting setting)
        {
            var val = (SettingPreset)setting;
            presetDropdown.dropdown.SetSelectedIndexForDataWithoutNotify((int)val.settingQualityLevel);
        }

        void UpdateVsyncToggle(ISetting setting)
        {
            var val = (VsyncSetting)setting;
            vsyncToggle.toggle.SetIsOnWithoutNotify(val.isOn);
        }

        void UpdateAntiAliasingDropdown(ISetting setting)
        {
            antiAliasDropdown.dropdown.SetSelectedIndexForDataWithoutNotify(setting);
        }

        void UpdateShadowQualityDropdown(ISetting setting)
        {
            shadowQualityDropdown.dropdown.SetSelectedIndexForDataWithoutNotify(setting);
        }

        void UpdateTextureQualityDropdown(ISetting setting)
        {
            textureQualityDropdown.dropdown.SetSelectedIndexForDataWithoutNotify(setting);
        }

        void UpdateResolutionDropdown(ISetting setting)
        {
            resolutionDropdown.dropdown.SetSelectedIndexForDataWithoutNotify(setting);
        }

        void UpdateDisplayTypeDropdown(ISetting setting)
        {
            displayTypeDropdown.dropdown.SetSelectedIndexForDataWithoutNotify(setting);
        }

        // Direct interaction with GraphicSettingContainer - Change settings on value change

        void OnPresetValueChanged(int index)
        {
            var presetIndex = (int)presetDropdown.dropdown.GetData(index).value;
            graphicSettingContainer.ChangePreset(graphicSettingContainer.GetPresetAt(presetIndex));
        }

        void OnResolutionValueChanged(int index)
        {
            var setting = (ResolutionSetting)resolutionDropdown.dropdown.GetData(index).value;
            graphicSettingContainer.ChangeSetting(setting);
        }

        void OnDisplayTypeValueChanged(int index)
        {
            var setting = (DisplayTypeSetting)displayTypeDropdown.dropdown.GetData(index).value;
            graphicSettingContainer.ChangeSetting(setting);
        }

        void OnVsyncValueChanged(bool value)
        {
            graphicSettingContainer.ChangeSetting(new VsyncSetting(value));
        }

        void OnAntiAliasValueChanged(int index)
        {
            var setting = (AntiAliasingSetting)antiAliasDropdown.dropdown.GetData(index).value;
            graphicSettingContainer.ChangeSetting(setting);
        }

        void OnShadowQualityValueChanged(int index)
        {
            var setting = (ShadowQualitySetting)shadowQualityDropdown.dropdown.GetData(index).value;
            graphicSettingContainer.ChangeSetting(setting);
        }

        void OnTextureQualityValueChanged(int index)
        {
            var setting = (TextureQualitySetting)textureQualityDropdown.dropdown.GetData(index).value;
            graphicSettingContainer.ChangeSetting(setting);
        }

        void OnSettingsLoaded(XIVSettings settings)
        {
            graphicSettingContainer?.RemoveListener(this);
            graphicSettingContainer = settings.GetContainer<GraphicSettingContainer>();
            graphicSettingContainer.AddListener(this);
            InitializeUIItems();
        }

        void InitializeUIItems()
        {
            var antiAliasingOptions = ListPool<AntiAliasingSetting>.Get();
            var shadowOptions = ListPool<ShadowQualitySetting>.Get();
            var textureOptions = ListPool<TextureQualitySetting>.Get();
            InitializePresetDropdown(antiAliasingOptions, shadowOptions, textureOptions);
            InitializeAntiAliasingDropdown(antiAliasingOptions);
            InitializeShadowDropdown(shadowOptions);
            InitializeTextureDropdown(textureOptions);
            InitializeResolutionOptions();
            InitializeDisplayType();

            ListPool<AntiAliasingSetting>.Release(antiAliasingOptions);
            ListPool<ShadowQualitySetting>.Release(shadowOptions);
            ListPool<TextureQualitySetting>.Release(textureOptions);

            var preset = graphicSettingContainer.GetSetting<SettingPreset>();
            UpdatePresetDropdown(preset);
            UpdateVsyncToggle(preset.GetSetting<VsyncSetting>());
            UpdateAntiAliasingDropdown(preset.GetSetting<AntiAliasingSetting>());
            UpdateShadowQualityDropdown(preset.GetSetting<ShadowQualitySetting>());
            UpdateTextureQualityDropdown(preset.GetSetting<TextureQualitySetting>());
            UpdateResolutionDropdown(preset.GetSetting<ResolutionSetting>());
            UpdateDisplayTypeDropdown(preset.GetSetting<DisplayTypeSetting>());
        }

        void InitializePresetDropdown(List<AntiAliasingSetting> antiAliasingOptions, List<ShadowQualitySetting> shadowOptions,
            List<TextureQualitySetting> textureOptions)
        {
            presetDropdown.dropdown.ClearOptions();
            var presetDropdownOptions = new List<DropdownOptionData<object>>();
            int presetCount = graphicSettingContainer.presetCount;
            for (int i = 0; i < presetCount; i++)
            {
                var preset = graphicSettingContainer.GetPresetAt(i);

                presetDropdownOptions.Add(new DropdownOptionData<object>(qualityOptionStrings[i], (int)preset.settingQualityLevel));

                var antiAliasing = preset.GetSetting<AntiAliasingSetting>();
                var shadow = preset.GetSetting<ShadowQualitySetting>();
                var texture = preset.GetSetting<TextureQualitySetting>();

                if (antiAliasingOptions.Contains(antiAliasing) == false) antiAliasingOptions.Add(antiAliasing);
                if (shadowOptions.Contains(shadow) == false) shadowOptions.Add(shadow);
                if (textureOptions.Contains(texture) == false) textureOptions.Add(texture);
            }

            presetDropdownOptions.Add(new DropdownOptionData<object>("Custom", (int)SettingQualityLevel.Custom));
            presetDropdown.dropdown.AddOptions(presetDropdownOptions);
        }

        void InitializeAntiAliasingDropdown(List<AntiAliasingSetting> antiAliasingOptions)
        {
            antiAliasDropdown.dropdown.ClearOptions();
            antiAliasingOptions.Sort((a, b) => a.antiAliasing > b.antiAliasing ? 1 : -1);

            var antiAliasingDropdownOptions = new List<DropdownOptionData<object>>();
            int antiAliasingOptionCount = antiAliasingOptions.Count;

            antiAliasingDropdownOptions.Add(new DropdownOptionData<object>("Off", antiAliasingOptions[0]));
            for (int i = 1; i < antiAliasingOptionCount; i++)
            {
                antiAliasingDropdownOptions.Add(new DropdownOptionData<object>(antiAliasingOptions[i].antiAliasing + "x MSAA", antiAliasingOptions[i]));
            }
            antiAliasDropdown.dropdown.AddOptions(antiAliasingDropdownOptions);
        }

        void InitializeShadowDropdown(List<ShadowQualitySetting> shadowOptions)
        {
            shadowQualityDropdown.dropdown.ClearOptions();

            var shadowDropdownOptions = new List<DropdownOptionData<object>>();
            int shadowOptionCount = shadowOptions.Count;
            for (int i = 0; i < shadowOptionCount; i++)
            {
                shadowDropdownOptions.Add(new DropdownOptionData<object>(qualityOptionStrings[i], shadowOptions[i]));
            }
            shadowQualityDropdown.dropdown.AddOptions(shadowDropdownOptions);
        }

        void InitializeTextureDropdown(List<TextureQualitySetting> textureOptions)
        {
            textureQualityDropdown.dropdown.ClearOptions();
            textureOptions.Sort((a, b) => a.masterTextureLimit < b.masterTextureLimit ? 1 : -1);

            var textureDropdownOptions = new List<DropdownOptionData<object>>();
            int textureOptionCount = textureOptions.Count;
            for (int i = 0; i < textureOptionCount; i++)
            {
                textureDropdownOptions.Add(new DropdownOptionData<object>(qualityOptionStrings[i], textureOptions[i]));
            }
            textureQualityDropdown.dropdown.AddOptions(textureDropdownOptions);
        }

        void InitializeResolutionOptions()
        {
            resolutionDropdown.dropdown.ClearOptions();

            var bindedDatas = new List<DropdownOptionData<object>>();
            var resolutions = Screen.resolutions;
            int length = resolutions.Length;
            for (int i = 0; i < length; i++)
            {
                var res = resolutions[i];
                var resText = res.width + "x" + res.height;
                bindedDatas.Add(new DropdownOptionData<object>(resText, new ResolutionSetting(res.width, res.height)));
            }
            resolutionDropdown.dropdown.AddOptions(bindedDatas);
        }

        void InitializeDisplayType()
        {
            displayTypeDropdown.dropdown.ClearOptions();
            var datas = new List<DropdownOptionData<object>>
            {
                new DropdownOptionData<object>("FullScreen", new DisplayTypeSetting(true)),
                new DropdownOptionData<object>("Windowed", new DisplayTypeSetting(false)),
            };
            displayTypeDropdown.dropdown.AddOptions(datas);
        }

        void ISettingListener.OnSettingChanged(SettingChange settingChange)
        {
            onSettingChangeLookup.TryGetValue(settingChange.settingType, out var value);
            value.Invoke(settingChange.to);

            btn_Undo.interactable = graphicSettingContainer.undoCount > 0;
            btn_Redo.interactable = graphicSettingContainer.redoCount > 0;
        }

        void ISettingListener.OnBeforeApply(ISettingContainer _) { }

        void ISettingListener.OnAfterApply(ISettingContainer _) { }
    }
}