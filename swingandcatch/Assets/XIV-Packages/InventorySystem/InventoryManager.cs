using TheGame.SaveSystems;
using TheGame.ScriptableObjects.Channels;
using UnityEngine;
using XIV.InventorySystem.ScriptableObjects;
using XIV.InventorySystem.ScriptableObjects.ChannelSOs;

namespace XIV.InventorySystem
{
    public class InventoryManager : MonoBehaviour, IInventoryListener, ISavable
    {
        [SerializeField] InventorySO inventorySO;
        [SerializeField] InventoryChannelSO inventoryLoadedChannel;
        [SerializeField] InventoryChangeChannelSO inventoryChangedChannel;
        [SerializeField] VoidChannelSO onSceneReady;

        Inventory inventory;

        void Awake() => inventory = inventorySO.GetInventory();
        void Start() => inventoryLoadedChannel.RaiseEvent(inventory);
        
        void OnEnable()
        {
            onSceneReady?.Register(OnSceneReady);
            inventory.AddListener(this);
        }

        void OnDisable()
        {
            onSceneReady?.Unregister(OnSceneReady);
            inventory.RemoveListener(this);
        }

        void OnSceneReady()
        {
            inventoryLoadedChannel.RaiseEvent(inventory);
        }

        void IInventoryListener.OnInventoryChanged(InventoryChange inventoryChange)
        {
            inventoryChangedChannel.RaiseEvent(inventoryChange);
        }

        #region Save

        [System.Serializable]
        struct SaveData
        {
            public ItemBase[] items;
            public int[] amounts;
        }
        
        object ISavable.GetSaveData()
        {
            int count = inventory.Count;
            ItemBase[] items = new ItemBase[count];
            int[] amounts = new int[count];
            for (int i = 0; i < count; i++)
            {
                ReadOnlyInventoryItem readOnlyInventoryItem = inventory[i];
                items[i] = readOnlyInventoryItem.Item;
                amounts[i] = readOnlyInventoryItem.Amount;
            }
            return new SaveData
            {
                items = items,
                amounts = amounts,
            };
        }

        void ISavable.LoadSaveData(object state)
        {
            SaveData saveData = (SaveData)state;
            if (saveData.items == null || saveData.items.Length == 0) return;

            for (int i = 0; i < inventory.SlotCount; i++)
            {
                int amount = int.MaxValue;
                inventory.RemoveAt(i, ref amount, false);
            }
            
            for (int i = 0; i < saveData.items.Length - 1; i++)
            {
                inventory.TryAdd(saveData.items[i], ref saveData.amounts[i], false);
            }

            int last = saveData.items.Length - 1;
            inventory.TryAdd(saveData.items[last], ref saveData.amounts[last], true);
        }
        
        #endregion
        
    }
}