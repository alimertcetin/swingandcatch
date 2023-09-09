using System.Collections.Generic;

namespace XIV_Packages.PCSettingSystems.Core
{
    public class XIVSettings
    {
        List<ISettingContainer> settingContainers;

        public int containerCount => settingContainers.Count;

        public XIVSettings()
        {
            settingContainers = new List<ISettingContainer>();
        }

        public T GetContainer<T>() where T : ISettingContainer
        {
            int count = settingContainers.Count;
            for (int i = 0; i < count; i++)
            {
                if (settingContainers[i] is T container) return container;
            }

            return default;
        }

        public bool AddContainer(ISettingContainer settingContainer)
        {
            if (settingContainers.Contains(settingContainer)) return false;
            settingContainers.Add(settingContainer);
            return true;
        }

        public bool RemoveContainer(ISettingContainer settingContainer)
        {
            return settingContainers.Remove(settingContainer);
        }

        public ISettingContainer GetContainerAt(int index)
        {
            return settingContainers[index];
        }
    }
}