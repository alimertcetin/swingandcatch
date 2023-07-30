using System;
using TheGame.AbilitySystems.Core;
using TheGame.Interfaces;

namespace TheGame.AbilitySystems.Abilities
{
    public class AttackAbility : IAbility
    {
        AbilityState abilityState;
        IAttackHandler attackHandler;
        IAbilityUser abilityUser;
        
        void IAbility.Initialize(IAbilityUser abilityUser)
        {
            this.abilityUser = abilityUser;
            attackHandler = abilityUser.GetComponent<IAttackHandler>();
        }

        void IAbility.PrepareForUse()
        {
            this.abilityUser.BeginUse(this);
            abilityState = AbilityState.Active;
        }

        void IAbility.Update()
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

        AbilityState IAbility.GetState() => abilityState;

        bool IAbility.IsAvailableToUse() => abilityState == AbilityState.None || abilityState == AbilityState.Inactive || attackHandler.CanAttack();
    }
}