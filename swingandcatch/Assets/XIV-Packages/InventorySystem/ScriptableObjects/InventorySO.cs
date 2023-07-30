using System;
using System.Collections.Generic;
using UnityEngine;

namespace XIV.InventorySystem.ScriptableObjects
{
    [System.Serializable]
    public struct ItemSOData
    {
        public ItemSO itemSO;
        public int amount;
    }
    
    [CreateAssetMenu(menuName = MenuPaths.BASE_MENU + nameof(InventorySO))]
    public class InventorySO : ScriptableObject
    {
        public int SlotCount;
        public List<ItemSOData> items;
#if UNITY_EDITOR
        [System.Serializable]
        struct RuntimeItemData
        {
            public string name;
            public ItemBase item;
            public int amount;
        }
        [SerializeField] List<RuntimeItemData> runtimeItems;
#endif

        public Inventory GetInventory()
        {
            var inventory = new Inventory(SlotCount);
            
            for (var i = 0; i < items.Count; i++)
            {
                ItemSO itemSO = items[i].itemSO;
                int amount = items[i].amount;
                if (amount <= 0)
                {
                    Debug.LogError(new InvalidOperationException("Amount cant be less than or equal to 0"));
                    break;
                }

                bool isAdded = inventory.TryAdd(itemSO.GetItem(), ref amount);
                if (!isAdded)
                {
                    Debug.LogError("Inventory is full! Couldnt add item at index : " + i);
                    break;
                }
            }
            
#if UNITY_EDITOR
            runtimeItems = new List<RuntimeItemData>(SlotCount);
            for (var i = 0; i < inventory.Count; i++)
            {
                ReadOnlyInventoryItem item = inventory[i];
                var runtimeItem = new RuntimeItemData
                {
                    name = item.Item.GetType().Name.Split('.')[^1], 
                    amount = item.Amount,
                    item = item.Item,
                };
                runtimeItems.Add(runtimeItem);
            }
            inventory.AddListener(new InventoryRuntimeListener(inventory, runtimeItems));
#endif
            
            return inventory;
        }

#if UNITY_EDITOR
        class InventoryRuntimeListener : IInventoryListener
        {
            Inventory inventory;
            List<RuntimeItemData> runtimeItems;
            
            public InventoryRuntimeListener(Inventory inventory, List<RuntimeItemData> runtimeItems)
            {
                this.inventory = inventory;
                this.runtimeItems = runtimeItems;
                Refresh();
            }
            
            void IInventoryListener.OnInventoryChanged(InventoryChange inventoryChange)
            {
                Refresh();
            }

            void Refresh()
            {
                runtimeItems.Clear();
                for (int i = 0; i < inventory.SlotCount; i++)
                {
                    ReadOnlyInventoryItem item = inventory[i];
                    var name = item.IsEmpty == false ? item.Item.GetType().Name.Split('.')[^1] : "Empty";
                    var amount = item.Amount;
                    var itemBase = item.Item;
                    var runtimeItem = new RuntimeItemData { name = name, amount = amount, item = itemBase, };
                    runtimeItems.Add(runtimeItem);
                }
            }
        }
#endif
    }
}