using System.Collections.Generic;

namespace XIV_Packages.PCSettingSystems.Core
{
    public interface ISettingContainer
    {
        /// <summary>
        /// Returns true if there is an unapplied change in <see cref="ISettingContainer"/>
        /// </summary>
        bool HasUnappliedChange();

        /// <summary>
        /// Returns the <typeparamref name="T"/> if found in <see cref="ISettingContainer"/>
        /// </summary>
        /// <typeparam name="T">The type of the setting</typeparam>
        T GetSetting<T>() where T : struct, ISetting;

        /// <summary>
        /// This will queue the change. To Apply the changes use <see cref="ApplyChanges"/>.
        /// </summary>
        /// <typeparam name="T">Setting Type</typeparam>
        /// <param name="newValue">New value of setting</param>
        /// <returns><see langword="true"/> if setting is changed, <see langword="false"/> otherwise. false means current value is equal to the <paramref name="newValue"/></returns>
        bool ChangeSetting<T>(T newValue) where T : struct, ISetting;

        /// <summary>
        /// Applies all pending changes to <see cref="ISettingContainer"/> instance.
        /// </summary>
        void ApplyChanges();

        /// <summary>
        /// Returns all current settings
        /// </summary>
        IEnumerable<ISetting> GetSettings();

        /// <summary>
        /// Returns all unapplied settings
        /// </summary>
        IEnumerable<SettingChange> GetUnappliedSettings();

        /// <summary>
        /// Retruns <see langword="true"/> if undo is successful
        /// </summary>
        bool Undo();

        /// <summary>
        /// Returns <see langword="true"/> if redo is successful
        /// </summary>
        bool Redo();

        /// <summary>
        /// Clears the undo history
        /// </summary>
        void ClearUndoHistory();

        bool AddListener(ISettingListener listener);
        bool RemoveListener(ISettingListener listener);
    }
}