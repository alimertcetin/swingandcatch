using System;
using UnityEngine;
using XIV.InventorySystem;
using XIV.InventorySystem.ScriptableObjects.ChannelSOs;

namespace TheGame.InventorySystems
{
    public class PlayerFSMInventoryContainer : MonoBehaviour, IInventoryContainer
    {
        [SerializeField] InventoryChannelSO onInventoryLoadedChannel;

        Inventory inventory;

        void OnEnable()
        {
            onInventoryLoadedChannel.Register(OnInventoryLoaded);
        }

        void OnDisable()
        {
            onInventoryLoadedChannel.Unregister(OnInventoryLoaded);
        }

        void OnInventoryLoaded(Inventory inventory)
        {
            this.inventory = inventory;
        }

        public Inventory GetInventory()
        {
            return inventory;
        }
    }
}