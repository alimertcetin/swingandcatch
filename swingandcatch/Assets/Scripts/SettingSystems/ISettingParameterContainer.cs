using TheGame.SaveSystems;

namespace TheGame.SettingSystems
{
    public interface ISettingParameterContainer : ISavable
    {
        SettingParameterType GetParameterType();
        bool SetParameter(int parameterNameHash, object newValue);
        SettingParameter GetParameter(int parameterNameHash);
    }
}