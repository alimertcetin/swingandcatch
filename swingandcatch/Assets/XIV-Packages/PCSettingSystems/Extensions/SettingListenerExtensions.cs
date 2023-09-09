using System;
using System.Collections.Generic;
using XIV_Packages.PCSettingSystems.Core;

namespace XIV_Packages.PCSettingSystems.Extensions
{
    public static class SettingListenerExtensions
    {
        public static void InformBeforeApply(this IList<ISettingListener> listeners, ISettingContainer container)
        {
            int count = listeners.Count;
            for (int i = 0; i < count; i++)
            {
                listeners[i].OnBeforeApply(container);
            }
        }

        public static void InformSettingChange(this IList<ISettingListener> listeners, SettingChange change)
        {
            int count = listeners.Count;
            for (int i = 0; i < count; i++)
            {
                listeners[i].OnSettingChanged(change);
            }
        }

        public static void InformAfterApply(this IList<ISettingListener> listeners, ISettingContainer container)
        {
            int count = listeners.Count;
            for (int i = 0; i < count; i++)
            {
                listeners[i].OnAfterApply(container);
            }
        }

        public static int IndexOfType(this IList<ISetting> settings, Type type)
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