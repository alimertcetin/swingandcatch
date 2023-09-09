using System;
using System.Collections.Generic;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Core.Commands;

namespace XIV_Packages.PCSettingSystems.Extras.SettingContainers
{
    public class GraphicSettingContainer : SettingContainerBase
    {
        public int presetCount => presets.Count;

        List<SettingPreset> presets;
        static readonly Type settingPresetType = typeof(SettingPreset);

        public GraphicSettingContainer(ISettingApplier settingApplier) : base(settingApplier)
        {
            presets = new List<SettingPreset>();
        }

        public override bool ChangeSetting<T>(T newValue)
        {
            var type = typeof(T);
            var index = IndexOfType(type);
            var currentValue = unappliedChanges.ContainsKey(type) ? unappliedChanges[type].to : settings[index];
            if (currentValue.Equals(newValue)) return false;
            commands.Do(new SettingChangeWithPresetCommand(index, unappliedChanges, newValue, listeners, settings, presets));
            return true;
        }

        /// <summary>
        /// Clears the settings list and adds settings that contained in <paramref name="preset"/>. This operation is not registered to undo system, so be cautious.
        /// </summary>
        public void InitializeSettings(SettingPreset preset)
        {
            settings.Clear();
            AddPreset(preset);
            settings.Add(preset);

            int count = preset.presetSettings.Count;
            for (int i = 0; i < count; i++)
            {
                settings.Add(preset.presetSettings[i]);
            }
        }

        public void AddPresets(IList<SettingPreset> presets)
        {
            int count = presets.Count;
            for (int i = 0; i < count; i++)
            {
                AddPreset(presets[i]);
            }
        }

        public void AddPreset(SettingPreset preset)
        {
            int count = presets.Count;
            for (int i = 0; i < count; i++)
            {
                if (SettingPreset.IsEqual(preset, presets[i].presetSettings))
                {
                    return;
                }
            }
            presets.Add(preset);
        }

        public bool RemovePreset(SettingPreset preset)
        {
            int count = presets.Count;
            for (int i = 0; i < count; i++)
            {
                if (SettingPreset.IsEqual(preset, presets[i].presetSettings))
                {
                    presets.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void ChangePreset(SettingPreset preset)
        {
            var index = IndexOfType(settingPresetType);
            bool containsKey = unappliedChanges.ContainsKey(settingPresetType);
            var currentPreset = (SettingPreset)(containsKey ? unappliedChanges[settingPresetType].to : settings[index]);

            if (SettingPreset.IsEqual(preset, currentPreset.presetSettings))
            {
                if (containsKey == false) return;

                unappliedChanges.Remove(settingPresetType);
                InformSettingChange(new SettingChange(index, currentPreset, preset));
                return;
            }

            commands.Do(new ChangeSettingPresetCommand(index, unappliedChanges, listeners, preset, settings));
        }

        public SettingPreset GetPresetAt(int index) => presets[index];

        void InformSettingChange(SettingChange change)
        {
            int count = listeners.Count;
            for (int i = 0; i < count; i++)
            {
                listeners[i].OnSettingChanged(change);
            }
        }
    }
}