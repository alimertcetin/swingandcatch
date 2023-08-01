using System;
using Newtonsoft.Json;
using TheGame.AbilitySystems.Core;
using TheGame.Interfaces;
using UnityEngine;
using XIV.Core.Utils;

namespace TheGame.AbilitySystems.Abilities
{
    [System.Serializable]
    public class DoubleJumpAbility : AbilityItem
    {
        [SerializeField, JsonIgnore] Timer cooldownTimer;
        IAbilityUser abilityUser;
        IMovementHandler movementHandler;
        AbilityState abilityState;

        public override void Initialize(IAbilityUser abilityUser)
        {
            this.abilityUser = abilityUser;
            movementHandler = abilityUser.GetComponent<IMovementHandler>();
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

        public override AbilityState GetState() => abilityState;

        public override bool IsAvailableToUse() => abilityState == AbilityState.None || abilityState == AbilityState.Inactive;
    }
}