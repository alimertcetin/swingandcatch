namespace XIV_Packages.PCSettingSystems.Core
{
    public interface ISettingApplier
    {
        bool AddApplyCommand<T>(ApplyCommand<T> command) where T : ISetting;
        bool RemoveApplyCommand<T>() where T : ISetting;
        void Apply(ISettingContainer settingContainer);
    }
}