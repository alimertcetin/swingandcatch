using System;
using System.Collections.Generic;
using UnityEngine.Pool;
using XIV_Packages.PCSettingSystems.Core.UndoSystems;
using XIV_Packages.PCSettingSystems.Extensions;

namespace XIV_Packages.PCSettingSystems.Core.Commands
{
    public struct SettingChangeWithPresetCommand : ICommand
    {
        public readonly int index;
        public readonly ISetting setting;

        public readonly Type settingType;
        public readonly Dictionary<Type, SettingChange> unappliedSettings;
        public readonly IList<ISettingListener> listeners;
        public readonly IList<ISetting> settings;
        public readonly IList<SettingPreset> presets;
        SettingChange presetChange;
        SettingChange settingChange;
        bool isPresetChanged;
        static readonly Type settingPresetType = typeof(SettingPreset);

        public SettingChangeWithPresetCommand(int index, Dictionary<Type, SettingChange> unappliedSettings, ISetting newValue,
            IList<ISettingListener> listeners, IList<ISetting> settings, IList<SettingPreset> presets)
        {
            this.index = index;
            this.unappliedSettings = unappliedSettings;
            this.settingType = newValue.GetType();
            this.setting = newValue;
            this.listeners = listeners;
            this.settings = settings;
            this.presets = presets;
            presetChange = default;
            settingChange = default;
            isPresetChanged = false;
        }

        void ICommand.Execute()
        {
            var currentValue = unappliedSettings.ContainsKey(settingType) ? unappliedSettings[settingType].to : settings[index];
            settingChange = new SettingChange(index, currentValue, setting);

            unappliedSettings.AddOrReplace(settingType, settingChange);

            listeners.InformSettingChange(settingChange);

            presetChange = ResolvePreset(out isPresetChanged);
            if (isPresetChanged == false) return;

            unappliedSettings.AddOrReplace(settingPresetType, presetChange);

            listeners.InformSettingChange(presetChange);
        }

        void ICommand.Unexecute()
        {
            settingChange.Reverse();
            unappliedSettings[settingType] = settingChange;

            listeners.InformSettingChange(settingChange);

            if (isPresetChanged == false) return;

            presetChange.Reverse();
            unappliedSettings[settingPresetType] = presetChange;

            listeners.InformSettingChange(presetChange);
        }

        SettingChange ResolvePreset(out bool isChanged)
        {
            bool containsKey = unappliedSettings.ContainsKey(settingPresetType);
            var presetIndex = settings.IndexOfType(settingPresetType);

            var currentPreset = (SettingPreset)(containsKey ? unappliedSettings[settingPresetType].to : settings[presetIndex]);

            var dic = DictionaryPool<Type, ISetting>.Get();
            foreach (SettingChange unappliedSetting in unappliedSettings.Values)
            {
                dic.Add(unappliedSetting.settingType, unappliedSetting.to);
            }

            int settingCount = settings.Count;
            for (int i = 0; i < settingCount; i++)
            {
                var setting = settings[i];
                dic.TryAdd(setting.GetType(), setting);
            }

            dic.Remove(settingPresetType);

            var correspondingPresetIndex = presets.GetCorrespondingPresetIndex(dic.Values);

            SettingChange change;
            if (correspondingPresetIndex == -1)
            {
                var newPreset = new SettingPreset(SettingQualityLevel.Custom, new List<ISetting>(dic.Values));
                change = new SettingChange(presetIndex, currentPreset, newPreset);
                isChanged = true;
            }
            else
            {
                var newPreset = presets[correspondingPresetIndex];
                if (SettingPreset.IsEqual(newPreset, currentPreset.presetSettings))
                {
                    change = default;
                    isChanged = false;
                }
                else
                {
                    change = new SettingChange(presetIndex, currentPreset, newPreset);
                    isChanged = true;
                }
            }
            DictionaryPool<Type, ISetting>.Release(dic);
            return change;
        }

        public override string ToString()
        {
            return settingType.Name;
        }
    }
}