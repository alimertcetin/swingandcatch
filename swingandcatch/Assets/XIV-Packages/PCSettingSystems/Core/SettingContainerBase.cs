using System;
using System.Collections.Generic;
using XIV_Packages.PCSettingSystems.Core.Commands;
using XIV_Packages.PCSettingSystems.Core.UndoSystems;

namespace XIV_Packages.PCSettingSystems.Core
{
    public abstract class SettingContainerBase : ISettingContainer
    {
        public int undoCount => commands.undoCount;
        public int redoCount => commands.redoCount;

        protected ISettingApplier settingApplier;
        protected List<ISetting> settings;
        protected List<ISettingListener> listeners;
        protected Dictionary<Type, SettingChange> unappliedChanges;
        protected UndoRedoStack<ICommand> commands;

        public SettingContainerBase(ISettingApplier settingApplier)
        {
            this.settingApplier = settingApplier;
            settings = new List<ISetting>();
            listeners = new List<ISettingListener>();
            unappliedChanges = new Dictionary<Type, SettingChange>();
            commands = new UndoRedoStack<ICommand>();
        }

        /// <summary>
        /// Clears the settings list and adds settings that contained in <paramref name="settingList"/>. This operation is not registered to undo system, so be cautious.
        /// </summary>
        public void InitializeSettings(IList<ISetting> settingList)
        {
            settings.Clear();
            settings.AddRange(settingList);
        }

        public virtual bool HasUnappliedChange() => unappliedChanges.Count > 0;

        public virtual T GetSetting<T>() where T : struct, ISetting => (T)settings[IndexOfType(typeof(T))];

        public virtual bool ChangeSetting<T>(T newValue) where T : struct, ISetting
        {
            var type = typeof(T);
            var index = IndexOfType(type);
            var currentValue = unappliedChanges.ContainsKey(type) ? unappliedChanges[type].to : settings[index];
            if (currentValue.Equals(newValue)) return false;
            commands.Do(new SettingChangeCommand(index, unappliedChanges, newValue, listeners, settings));
            return true;
        }

        public virtual void ApplyChanges()
        {
            commands.Do(new ApplySettingChangesCommand(this, settingApplier, listeners, settings, unappliedChanges));
        }

        public IEnumerable<ISetting> GetSettings() => settings;

        public IEnumerable<SettingChange> GetUnappliedSettings() => unappliedChanges.Values;


        public virtual bool Undo()
        {
            if (commands.undoCount == 0) return false;
            commands.Undo();
            return true;
        }

        public virtual bool Redo()
        {
            if (commands.redoCount == 0) return false;
            commands.Redo();
            return true;
        }

        public virtual void ClearUndoHistory() => commands.Clear();

        public virtual bool AddListener(ISettingListener listener)
        {
            if (listeners.Contains(listener)) return false;
            listeners.Add(listener);
            return true;
        }

        public virtual bool RemoveListener(ISettingListener listener) => listeners.Remove(listener);

        protected int IndexOfType(Type type)
        {
            int count = settings.Count;
            for (int i = 0; i < count; i++)
            {
                if (settings[i].GetType() == type) return i;
            }
            return -1;
        }
    }
}