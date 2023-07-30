using System;
using TheGame.AbilitySystems.Core;
using TheGame.Interfaces;
using UnityEngine;
using XIV.Core.Utils;

namespace TheGame.AbilitySystems.Abilities
{
    [System.Serializable]
    public class DoubleJumpAbility : IAbility
    {
        [SerializeField] Timer cooldownTimer;
        IAbilityUser abilityUser;
        IMovementHandler movementHandler;
        AbilityState abilityState;

        public DoubleJumpAbility() { }

        public DoubleJumpAbility(DoubleJumpAbility other)
        {
            this.cooldownTimer = other.cooldownTimer;
            this.abilityUser = other.abilityUser;
            this.movementHandler = other.movementHandler;
            this.abilityState = other.abilityState;
        }

        void IAbility.Initialize(IAbilityUser abilityUser)
        {
            this.abilityUser = abilityUser;
            movementHandler = abilityUser.GetComponent<IMovementHandler>();
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
                    if (movementHandler.CheckIsTouching(1 << PhysicsConstants.GroundLayer))
                    {
                        abilityState = AbilityState.Cooldown;
                        abilityUser.EndUse(this);
                    }

                    break;
                case AbilityState.Cooldown:
                    if (cooldownTimer.Update(Time.deltaTime))
                    {
                        abilityState = AbilityState.Inactive;
                        cooldownTimer.Restart();
                    }

                    break;
                case AbilityState.Inactive: return;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        AbilityState IAbility.GetState() => abilityState;

        bool IAbility.IsAvailableToUse() => abilityState == AbilityState.None || abilityState == AbilityState.Inactive;
    }
}