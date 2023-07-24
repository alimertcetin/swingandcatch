namespace TheGame.SettingSystems
{
    public interface ISettingsListener
    {
        void OnSettingsChanged(SettingParameter changedParameter);
    }
}