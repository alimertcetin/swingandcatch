using TheGame.UISystems.Core;

namespace TheGame.UISystems.SceneLoading
{
    public abstract class LoadingUIBase : PageUI
    {
        public abstract void UpdateProgressBar(float value);
    }
}