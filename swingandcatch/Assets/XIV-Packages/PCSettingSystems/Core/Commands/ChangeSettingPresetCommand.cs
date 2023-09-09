using System;
using System.Collections.Generic;
using XIV_Packages.PCSettingSystems.Core.UndoSystems;
using XIV_Packages.PCSettingSystems.Extensions;

namespace XIV_Packages.PCSettingSystems.Core.Commands
{
    public struct ChangeSettingPresetCommand : ICommand
    {
        int index;
        Dictionary<Type, SettingChange> unappliedChanges;
        IList<ISettingListener> listeners;
        SettingPreset preset;
        IList<ISetting> settings;
        SettingChange previousPresetChange;
        static readonly Type settingPresetType = typeof(SettingPreset);

        public ChangeSettingPresetCommand(int index, Dictionary<Type, SettingChange> unappliedSettings, IList<ISettingListener> listeners,
            SettingPreset preset, IList<ISetting> settings)
        {
            this.index = index;
            unappliedChanges = unappliedSettings;
            this.listeners = listeners;
            this.preset = preset;
            this.settings = settings;
            previousPresetChange = default;
        }

        void ICommand.Execute()
        {
            int count = preset.presetSettings.Count;
            for (int i = 0; i < count; i++)
            {
                ISetting setting = preset.presetSettings[i];
                var type = setting.GetType();
                var hasKey = unappliedChanges.ContainsKey(type);
                var settingIndex = settings.IndexOfType(type);
                var currentSetting = hasKey ? unappliedChanges[type].to : settings[settingIndex];
                var change = new SettingChange(settingIndex, currentSetting, setting);
                unappliedChanges.AddOrReplace(type, change);

                listeners.InformSettingChange(change);
            }

            var previousPreset = (SettingPreset)(unappliedChanges.ContainsKey(settingPresetType) ? unappliedChanges[settingPresetType].to : settings[index]);

            previousPresetChange = new SettingChange(index, previousPreset, preset);
            unappliedChanges.AddOrReplace(settingPresetType, previousPresetChange);

            listeners.InformSettingChange(previousPresetChange);
        }

        void ICommand.Unexecute()
        {
            previousPresetChange.Reverse();
            var preset = (SettingPreset)previousPresetChange.to;
            int count = preset.presetSettings.Count;
            for (int i = 0; i < count; i++)
            {
                ISetting setting = preset.presetSettings[i];
                var type = setting.GetType();
                var settingIndex = settings.IndexOfType(type);
                SettingChange change = unappliedChanges.ContainsKey(type) ?
                    new SettingChange(settingIndex, type, unappliedChanges[type].to, setting) :
                    new SettingChange(settingIndex, type, settings[settingIndex], setting);
                unappliedChanges.AddOrReplace(type, change);

                listeners.InformSettingChange(change);
            }

            unappliedChanges.AddOrReplace(settingPresetType, previousPresetChange);

            listeners.InformSettingChange(previousPresetChange);
        }
    }
}