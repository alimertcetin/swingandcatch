using System.Collections.Generic;
using TheGame.PlayerSystems;
using UnityEngine;

namespace TheGame.AbilitySystems.Core
{
    public class PlayerAbilityHandler : MonoBehaviour, IAbilityHandler
    {
        List<IAbility> abilities = new List<IAbility>();
        List<IAbility> activeAbilities = new List<IAbility>();
        List<IAbility> cooldownAbilities = new List<IAbility>();
        PlayerFSM playerFSM;

        void Awake()
        {
            playerFSM = GetComponent<PlayerFSM>();
        }

        void Update()
        {
            cooldownAbilities.RemoveAll(RemoveCooldownAbility);
            activeAbilities.RemoveAll(RemoveActiveAbility);

            int activeCount = activeAbilities.Count;
            for (int i = 0; i < activeCount; i++)
            {
                activeAbilities[i].Update();
            }

            var cooldownCount = cooldownAbilities.Count;
            for (int i = 0; i < cooldownCount; i++)
            {
                cooldownAbilities[i].Update();
            }
        }

        static bool RemoveCooldownAbility(IAbility ability)
        {
            return ability.GetState() != AbilityState.Cooldown;
        }

        bool RemoveActiveAbility(IAbility ability)
        {
            var state = ability.GetState();
            bool result = ability.GetState() != AbilityState.Active;
            if (result && state == AbilityState.Cooldown) cooldownAbilities.Add(ability);
            return result;
        }

        bool IAbilityHandler.HasActiveAbility() => activeAbilities.Count > 0;
        bool IAbilityHandler.AddAbility(IAbility ability)
        {
            if (abilities.Contains(ability)) return false;
            abilities.Add(ability);
            ability.Initialize(playerFSM);
            return true;
        }

        bool IAbilityHandler.RemoveAbility(IAbility ability)
        {
            return abilities.Remove(ability);
        }

        void IAbilityHandler.UseAbility(IAbility ability)
        {
            if (abilities.Contains(ability) == false || ability.IsAvailableToUse() == false) return;
            ability.PrepareForUse();
            activeAbilities.Add(ability);
        }

        T IAbilityHandler.GetAbility<T>()
        {
            int count = abilities.Count;
            for (int i = 0; i < count; i++)
            {
                if (abilities[i] is T a) return a;
            }

            return default;
        }
    }
}