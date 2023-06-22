using TheGame.UISystems.Core;

namespace TheGame.UISystems.SceneLoading
{
    public abstract class LoadingUIBase : GameUI
    {
        public abstract void UpdateProgressBar(float value);
    }
}