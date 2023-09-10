namespace TheGame.Assets.Scripts.InteractionSystems
{
    public interface IInteractable
    {
        bool IsInInteraction { get; }
        InteractionSettings GetInteractionSettings();
        bool IsAvailableForInteraction();
        void Interact(IInteractor interactor);
        string GetInteractionString();
        InteractionPositionData GetInteractionPositionData(IInteractor interactor);
    }
}
