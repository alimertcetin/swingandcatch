using System;
using TheGame.AbilitySystems.Core;
using TheGame.Interfaces;

namespace TheGame.AbilitySystems.Abilities
{
    [System.Serializable]
    public class AttackAbility : AbilityItem, IAbility
    {
        AbilityState abilityState;
        IAttackHandler attackHandler;
        IAbilityUser abilityUser;
        
        public override void Initialize(IAbilityUser abilityUser)
        {
            this.abilityUser = abilityUser;
            attackHandler = abilityUser.GetComponent<IAttackHandler>();
            abilityState = default;
        }

        public override void PrepareForUse()
        {
            this.abilityUser.BeginUse(this);
            abilityState = AbilityState.Active;
        }

        public override void Update()
        {
            switch (abilityState)
            {
                case AbilityState.None: return;
                case AbilityState.Active:
                    attackHandler.Attack();
                    abilityState = AbilityState.Cooldown;
                    abilityUser.EndUse(this);
                    break;
                case AbilityState.Cooldown:
                    if (attackHandler.CanAttack())
                    {
                        abilityState = AbilityState.Inactive;
                    }
                    break;
                case AbilityState.Inactive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override AbilityState GetState() => abilityState;

        public override bool IsAvailableToUse() => abilityState == AbilityState.None || abilityState == AbilityState.Inactive || attackHandler.CanAttack();
    }
}