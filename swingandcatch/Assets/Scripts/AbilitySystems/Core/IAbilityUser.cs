namespace TheGame.AbilitySystems.Core
{
    public interface IAbilityUser
    {
        void BeginUse(IAbility ability);
        void EndUse(IAbility ability);
        // MonoBehaviour already has this signature, This allows us to remove some restrictions from IAbility and IAbilityHandler
        // The thing is how a non-MonoBehaviour class will handle this?
        T GetComponent<T>();
    }
}