using TheGame.InventorySystems.Items;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using TMPro;
using UnityEngine;
using XIV.Core.Utils;
using XIV.Core.TweenSystem;
using XIV.InventorySystem;
using XIV.InventorySystem.ScriptableObjects.ChannelSOs;

namespace TheGame.UISystems
{
    public class HudCoinPageUI : PageUI
    {
        [SerializeField] RectTransform coinUIItemRect;
        [SerializeField] TMP_Text coinText;
        [SerializeField] InventoryChannelSO inventoryLoadedChannel;
        [SerializeField] InventoryChangeChannelSO inventoryChangedChannel;
        public Vector3 coinUIItemRectPosition => coinUIItemRect.position;
        int totalCoins;
        Inventory inventory;

        void OnEnable()
        {
            inventoryLoadedChannel.Register(OnInventoryLoaded);
            inventoryChangedChannel.Register(OnInventoryChanged);
        }

        void OnDisable()
        {
            inventoryLoadedChannel.Unregister(OnInventoryLoaded);
            inventoryChangedChannel.Unregister(OnInventoryChanged);
        }

        void OnInventoryLoaded(Inventory inventory)
        {
            this.inventory = inventory;
            RefreshCoinDisplay();
        }

        void OnInventoryChanged(InventoryChange inventoryChange)
        {
            RefreshCoinDisplay();
        }

        void RefreshCoinDisplay()
        {
            int amount = 0;
            var coinItems = inventory.GetItemsOfType<CoinItem>((_) => true);
            for (var i = 0; i < coinItems.Count; i++)
            {
                ReadOnlyInventoryItem readOnlyInventoryItem = coinItems[i];
                amount += readOnlyInventoryItem.Amount;
            }
            ChangeDisplayAmount(amount);
        }

        void ChangeDisplayAmount(int newAmount)
        {
            coinText.text = newAmount.ToString();
            coinUIItemRect.CancelTween(false);
            var localScale = coinUIItemRect.localScale;
            coinUIItemRect.XIVTween()
                .Scale(localScale, localScale + Vector3.one * 0.25f, 0.1f, EasingFunction.EaseInOutCubic)
                .OnComplete(() =>
                {
                    if (coinUIItemRect == null) return;
                    var currentScale = coinUIItemRect.localScale;
                    var diff = Vector3.Distance(currentScale, Vector3.one);
                    coinUIItemRect.XIVTween()
                        .Scale(currentScale, Vector3.one, diff * 0.5f, EasingFunction.EaseOutElastic)
                        .Start();
                })
                .Start();
        }
    }
}