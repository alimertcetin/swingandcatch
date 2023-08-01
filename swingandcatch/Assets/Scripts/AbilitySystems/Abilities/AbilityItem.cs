using TheGame.AbilitySystems.Core;
using XIV.InventorySystem;

namespace TheGame.AbilitySystems.Abilities
{
    [System.Serializable]
    public abstract class AbilityItem : ItemBase, IAbility
    {
        public abstract void Initialize(IAbilityUser abilityUser);

        public abstract void PrepareForUse();

        public abstract void Update();

        public abstract AbilityState GetState();

        public abstract bool IsAvailableToUse();
    }
}