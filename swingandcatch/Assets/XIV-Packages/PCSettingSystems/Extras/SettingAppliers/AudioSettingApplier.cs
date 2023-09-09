using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.SettingApplyCommands.AudioCommands;
using XIV_Packages.PCSettingSystems.Extras.SettingDatas.AudioDatas;

namespace XIV_Packages.PCSettingSystems.Extras.SettingAppliers
{
    public class AudioSettingApplier : ISettingApplier
    {
        AudioSettingApplyCommand audioSettingApplyCommand;

        bool ISettingApplier.AddApplyCommand<T>(ApplyCommand<T> command)
        {
            if (command is not AudioSettingApplyCommand audioSettingApplyCommand) return false;
            this.audioSettingApplyCommand = audioSettingApplyCommand;
            return true;
        }

        bool ISettingApplier.RemoveApplyCommand<T>()
        {
            bool isNull = audioSettingApplyCommand == null;
            audioSettingApplyCommand = null;
            return isNull == false;
        }

        void ISettingApplier.Apply(ISettingContainer settingContainer)
        {
            foreach (ISetting setting in settingContainer.GetSettings())
            {
                if (setting is IAudioSetting audioSetting)
                {
                    audioSettingApplyCommand.Apply(setting);
                }
#if UNITY_EDITOR
                else
                {
                    UnityEngine.Debug.LogWarning("There is no command added for " + setting.GetType().Name);
                }
#endif
            }
        }
    }
}