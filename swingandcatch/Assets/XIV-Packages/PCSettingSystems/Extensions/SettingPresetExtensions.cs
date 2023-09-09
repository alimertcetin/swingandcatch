using System.Collections.Generic;
using XIV_Packages.PCSettingSystems.Core;

namespace XIV_Packages.PCSettingSystems.Extensions
{
    public static class SettingPresetExtensions
    {
        public static int GetCorrespondingPresetIndex(this IList<SettingPreset> presets, IEnumerable<ISetting> settings)
        {
            int count = presets.Count;
            for (int i = 0; i < count; i++)
            {
                if (SettingPreset.IsEqual(presets[i], settings)) return i;
            }

            return -1;
        }
    }
}