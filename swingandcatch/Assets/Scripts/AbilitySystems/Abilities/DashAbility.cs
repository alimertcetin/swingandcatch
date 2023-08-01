using System;
using Newtonsoft.Json;
using TheGame.AbilitySystems.Core;
using TheGame.Interfaces;
using UnityEngine;
using XIV.Core.Utils;

namespace TheGame.AbilitySystems.Abilities
{
    [System.Serializable]
    public class DashAbility : AbilityItem
    {
        [SerializeField, JsonIgnore] float distance = 2f;
        [SerializeField, JsonIgnore] Timer dashTimer = new Timer(0.5f);
        [SerializeField, JsonIgnore] Timer coolDownTimer = new Timer(5f);

        IAbilityUser abilityUser;
        IMovementHandler movementHandler;
        Transform userTransform;
        
        Vector3 direction;
        Vector3 initialPosition;
        AbilityState abilityState;
        
        public override void Initialize(IAbilityUser abilityUser)
        {
            this.abilityUser = abilityUser;
            movementHandler = abilityUser.GetComponent<IMovementHandler>();
            userTransform = abilityUser.GetComponent<Transform>();
            abilityState = default;
        }

        public override void PrepareForUse()
        {
            this.direction = userTransform.forward;
            dashTimer.Restart();
            coolDownTimer.Restart();
            initialPosition = userTransform.position;
            abilityState = AbilityState.Active;
            abilityUser.BeginUse(this);
        }

        public override void Update()
        {
            switch (abilityState)
            {
                case AbilityState.None: return;
                case AbilityState.Active:
                    UpdateActive();
                    break;
                case AbilityState.Cooldown:
                    UpdateCooldown();
                    break;
                case AbilityState.Inactive:
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override AbilityState GetState()
        {
            return abilityState;
        }

        public override bool IsAvailableToUse() => abilityState == AbilityState.Inactive || abilityState == AbilityState.None;

        void UpdateActive()
        {
            dashTimer.Update(Time.deltaTime);
            var targetPos = Vector3.Lerp(initialPosition, initialPosition + direction * distance, dashTimer.NormalizedTime);
            if (movementHandler.Move(targetPos) && dashTimer.IsDone == false) return;

            Debug.Log("Dash Done");
            abilityUser.EndUse(this);
            abilityState = AbilityState.Cooldown;
        }

        void UpdateCooldown()
        {
            if (coolDownTimer.Update(Time.deltaTime))
            {
                Debug.Log("Dash Cooldown Done");
                abilityState = AbilityState.Inactive;
            }
        }
    }
}