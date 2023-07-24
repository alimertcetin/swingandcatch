using System;

namespace TheGame.SettingSystems
{
    public struct SettingParameter
    {
        public SettingParameterType settingParameterType;
        public string name;
        public int nameHash;
        public object value;

        public SettingParameter(SettingParameterType settingParameterType, string name, object value)
        {
            this.settingParameterType = settingParameterType;
            this.name = name;
            this.nameHash = name.GetHashCode();
            this.value = value;
        }

        public T ReadValue<T>() => ReadValue<T>(this);

        public static T ReadValue<T>(SettingParameter settingParameter)
        {
            try
            {
                return (T)settingParameter.value;
            }
            catch (InvalidCastException e)
            {
                UnityEngine.Debug.LogWarning($"Couldn't cast {typeof(T)} to {settingParameter.name}. Parameter value type is {settingParameter.value.GetType()}. Current value is {settingParameter.value}. {e}");
                return default;
            }
        }
    }
}