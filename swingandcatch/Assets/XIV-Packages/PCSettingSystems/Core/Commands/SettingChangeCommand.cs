using System;
using System.Collections.Generic;
using XIV_Packages.PCSettingSystems.Core.UndoSystems;
using XIV_Packages.PCSettingSystems.Extensions;

namespace XIV_Packages.PCSettingSystems.Core.Commands
{
    public struct SettingChangeCommand : ICommand
    {
        public readonly int index;
        public readonly ISetting setting;

        public readonly Type settingType;
        public readonly Dictionary<Type, SettingChange> unappliedSettings;
        public readonly IList<ISettingListener> listeners;
        public readonly IList<ISetting> settings;
        SettingChange settingChange;

        public SettingChangeCommand(int index, Dictionary<Type, SettingChange> unappliedSettings, ISetting newValue,
            IList<ISettingListener> listeners, IList<ISetting> settings)
        {
            this.index = index;
            this.unappliedSettings = unappliedSettings;
            this.settingType = newValue.GetType();
            this.setting = newValue;
            this.listeners = listeners;
            this.settings = settings;
            settingChange = default;
        }

        void ICommand.Execute()
        {
            var currentValue = unappliedSettings.ContainsKey(settingType) ? unappliedSettings[settingType].to : settings[index];
            settingChange = new SettingChange(index, currentValue, setting);
            unappliedSettings.AddOrReplace(settingType, settingChange);

            listeners.InformSettingChange(settingChange);
        }

        void ICommand.Unexecute()
        {
            settingChange.Reverse();
            unappliedSettings[settingType] = settingChange;

            listeners.InformSettingChange(settingChange);
        }

        public override string ToString()
        {
            return settingType.Name;
        }
    }
}