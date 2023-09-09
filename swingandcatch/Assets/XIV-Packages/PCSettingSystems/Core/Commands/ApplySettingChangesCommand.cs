using System;
using System.Collections.Generic;
using XIV_Packages.PCSettingSystems.Core.UndoSystems;
using XIV_Packages.PCSettingSystems.Extensions;

namespace XIV_Packages.PCSettingSystems.Core.Commands
{
    public readonly struct ApplySettingChangesCommand : ICommand
    {
        public readonly ISettingContainer container;
        public readonly ISettingApplier settingApplier;
        public readonly IList<ISettingListener> listeners;
        public readonly IList<ISetting> settings;
        public readonly Dictionary<Type, SettingChange> unappliedChanges;

        public readonly List<SettingChange> changeListUnapplied; // used for reconstructor the unapplied changes
        public readonly List<SettingChange> changeListSettings; // used for reconstructor the actual settings

        static readonly List<Type> buffer = new(); // Used for clearing the unchanged values from unappliedChanges

        public ApplySettingChangesCommand(ISettingContainer container, ISettingApplier settingApplier, IList<ISettingListener> listeners, IList<ISetting> settings,
            Dictionary<Type, SettingChange> unappliedChanges)
        {
            this.container = container;
            this.settingApplier = settingApplier;
            this.listeners = listeners;
            this.settings = settings;
            this.unappliedChanges = unappliedChanges;
            changeListUnapplied = new List<SettingChange>();
            changeListSettings = new List<SettingChange>();
        }

        void ICommand.Execute()
        {
            RemoveEqualValues();

            listeners.InformBeforeApply(container);

            foreach (var unappliedChange in unappliedChanges.Values)
            {
                var settingChange = new SettingChange(unappliedChange.index, unappliedChange.settingType, settings[unappliedChange.index], unappliedChange.to);
                changeListUnapplied.Add(unappliedChange);
                changeListSettings.Add(settingChange);
                settings[settingChange.index] = settingChange.to;
            }

            unappliedChanges.Clear();
            settingApplier.Apply(container);

            listeners.InformAfterApply(container);
        }

        void ICommand.Unexecute()
        {
            listeners.InformAfterApply(container); // TODO : Not sure I should reverse this callbacks

            int count = changeListSettings.Count;
            for (int i = 0; i < count; i++)
            {
                var settingChange = changeListSettings[i];
                settings[settingChange.index] = settingChange.from;

                SettingChange unappliedChange = changeListUnapplied[i];
                unappliedChanges.AddOrReplace(unappliedChange.settingType, unappliedChange);
            }

            changeListUnapplied.Clear();
            changeListSettings.Clear();
            settingApplier.Apply(container);

            listeners.InformBeforeApply(container);
        }

        void RemoveEqualValues()
        {
            buffer.Clear();
            foreach (var settingChange in unappliedChanges.Values)
            {
                if (settingChange.from.Equals(settingChange.to)) buffer.Add(settingChange.settingType);
            }
            unappliedChanges.RemoveKeys(buffer);
        }
    }
}