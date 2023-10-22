using System.Collections.Generic;
using TheGame.AbilitySystems.Abilities;
using TheGame.AbilitySystems.Core;
using UnityEngine;
using XIV.InventorySystem;
using XIV.InventorySystem.ScriptableObjects.ChannelSOs;

namespace TheGame.PlayerSystems
{
    public class PlayerFSMAbilityHandler : MonoBehaviour, IAbilityHandler
    {
        [SerializeField] InventoryChannelSO abilityInventoryLoadedChannel;
        
        List<IAbility> activeAbilities = new List<IAbility>();
        List<IAbility> cooldownAbilities = new List<IAbility>();
        Inventory abilityInventory;
        PlayerFSM playerFSM;

        void Awake()
        {
            playerFSM = GetComponent<PlayerFSM>();
        }

        void OnEnable()
        {
            abilityInventoryLoadedChannel.Register(OnAbilityInventoryLoaded);
        }

        void OnDisable()
        {
            abilityInventoryLoadedChannel.Unregister(OnAbilityInventoryLoaded);
        }

        void Update()
        {
            RemoveCooldownAbility();
            RemoveActiveAbility();

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

        void OnAbilityInventoryLoaded(Inventory abilityInventory)
        {
            this.abilityInventory = abilityInventory;
            for (int i = 0; i < abilityInventory.SlotCount; i++)
            {
                if (abilityInventory[i].IsEmpty) continue;
                ((IAbility)abilityInventory[i].Item).Initialize(playerFSM);
            }
        }

        void RemoveCooldownAbility()
        {
            int count = cooldownAbilities.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                if (cooldownAbilities[i].GetState() != AbilityState.Cooldown)
                {
                    cooldownAbilities.RemoveAt(i);
                }
            }
        }

        void RemoveActiveAbility()
        {
            int count = activeAbilities.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                var ability = activeAbilities[i];
                var state = ability.GetState();
                if (state != AbilityState.Active)
                {
                    activeAbilities.RemoveAt(i);
                    if (state == AbilityState.Cooldown)
                    {
                        cooldownAbilities.Add(ability);
                    }
                }
            }
        }

        bool IAbilityHandler.HasActiveAbility()
        {
            return activeAbilities.Count > 0;
        }

        bool IAbilityHandler.AddAbility(IAbility ability)
        {
            if (ability is not AbilityItem abilityItem || abilityInventory.Contains(abilityItem)) return false;
            int amount = 1;
            var isAdded = abilityInventory.TryAdd(abilityItem, ref amount);
            if (isAdded) ability.Initialize(playerFSM);
            return isAdded;
        }

        bool IAbilityHandler.RemoveAbility(IAbility ability)
        {
            if (ability is not AbilityItem abilityItem) return false;
            int amount = 1;
            abilityInventory.Remove(abilityItem, ref amount);
            return amount == 0;
        }

        void IAbilityHandler.UseAbility(IAbility ability)
        {
            if (ability is not AbilityItem abilityItem) return;
            
            if (abilityInventory.Contains(abilityItem) == false || ability.IsAvailableToUse() == false) return;
            ability.PrepareForUse();
            activeAbilities.Add(ability);
        }

        T IAbilityHandler.GetAbility<T>()
        {
            if (abilityInventory == null) return default;
            var items = abilityInventory.GetItemsOfType<AbilityItem>((item) => item is T);
            return (items[0].Item is T item) ? item : default;
        }
    }
}