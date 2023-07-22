using System;
using TheGame.UISystems.Core;

namespace TheGame.UISystems.SceneLoading
{
    public abstract class LoadingUIBase : GameUI
    {
        public event Action<LoadingUIBase> onUIOpened;
        public event Action<LoadingUIBase> onUIClosed;
        
        public abstract void UpdateProgressBar(float value);

        protected override void OnUIActivated()
        {
            onUIOpened?.Invoke(this);
        }

        protected override void OnUIDeactivated()
        {
            onUIClosed?.Invoke(this);
        }
    }
}