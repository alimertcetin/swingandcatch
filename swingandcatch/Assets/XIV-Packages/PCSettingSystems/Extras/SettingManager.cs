using UnityEngine;
using XIV_Packages.PCSettingSystems.Core;

namespace XIV_Packages.PCSettingSystems.Extras
{
    public abstract class SettingManager : MonoBehaviour
    {
        public abstract void InitializeContainer();
        public abstract ISettingContainer GetContainer();
    }
}