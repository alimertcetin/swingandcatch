using System;
using UnityEngine;

namespace XIV.InventorySystem.ScriptableObjects.ChannelSOs
{
    [CreateAssetMenu(menuName = MenuPaths.CHANNELS_MENU + nameof(ItemBaseChannelSO))]
    public class ItemBaseChannelSO : ScriptableObject
    {
        Action<ItemBase> action;

        public void Register(Action<ItemBase> action)
        {
            this.action += action;
        }

        public void Unregister(Action<ItemBase> action)
        {
            this.action -= action;
        }
        
        public void RaiseEvent(ItemBase item)
        {
            action?.Invoke(item);
        }
    }
}