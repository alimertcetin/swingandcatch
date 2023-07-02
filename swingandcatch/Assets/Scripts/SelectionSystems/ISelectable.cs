namespace TheGame.SelectionSystems
{
    public interface ISelectable
    {
        SelectionSettings GetSelectionSettings();
        void OnSelect();
        void OnDeselect();
    }
}







