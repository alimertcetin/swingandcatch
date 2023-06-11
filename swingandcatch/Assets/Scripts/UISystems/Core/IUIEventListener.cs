namespace TheGame.UISystems.Core
{
    public interface IUIEventListener
    {
        void OnShowUI(GameUI ui);
        void OnHideUI(GameUI ui);
    }

}