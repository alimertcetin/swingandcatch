namespace TheGame.AbilitySystems.Core
{
    public interface IAbility
    {
        void Initialize(IAbilityUser abilityUser);
        void PrepareForUse();
        void Update();
        AbilityState GetState();
        bool IsAvailableToUse();
    }
}