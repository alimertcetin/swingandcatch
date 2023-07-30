namespace TheGame.AbilitySystems.Core
{
    public interface IAbilityHandler
    {
        T GetAbility<T>() where T : IAbility;
        void UseAbility(IAbility ability);
        bool HasActiveAbility();
        bool AddAbility(IAbility ability);
        bool RemoveAbility(IAbility ability);
    }
}