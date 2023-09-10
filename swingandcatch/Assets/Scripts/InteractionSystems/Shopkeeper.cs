using TheGame.Assets.Scripts.InteractionSystems;
using TheGame.InventorySystems.Items;
using TheGame.UISystems.Core;
using TheGame.UISystems.Shop;
using UnityEngine;
using XIV.InventorySystem;
using XIV.InventorySystem.ScriptableObjects;
using XIV.InventorySystem.ScriptableObjects.ChannelSOs;

namespace TheGame.InteractionSystems
{
    public class Shopkeeper : MonoBehaviour, IInteractable, IShopAnchor, IUIEventListener
    {
        [SerializeField] InventoryChannelSO onPlayerItemInventoryLoaded;
        [SerializeField] InventorySO shopKeeperInventorySO;
        public bool IsInInteraction { get; private set; }

        Inventory shopKeeperInventory;

        Inventory interactorInventory;
        IInteractor interactor;

        void Awake()
        {
            shopKeeperInventory = shopKeeperInventorySO.GetInventory();
        }

        void OnEnable()
        {
            onPlayerItemInventoryLoaded.Register(OnPlayerItemInventoryLoaded);
        }

        void OnDisable()
        {
            onPlayerItemInventoryLoaded.Unregister(OnPlayerItemInventoryLoaded);
        }

        void OnPlayerItemInventoryLoaded(Inventory playerItemInventory)
        {
            interactorInventory = playerItemInventory;
        }

        InteractionPositionData IInteractable.GetInteractionPositionData(IInteractor interactor)
        {
            return new InteractionPositionData
            {
                startPos = ((MonoBehaviour)interactor).transform.position,
                targetPosition = transform.position,
                targetForwardDirection = transform.forward,
            };
        }

        InteractionSettings IInteractable.GetInteractionSettings()
        {
            return new InteractionSettings
            {
                suspendMovement = true,
                disableInteractionKey = true,
            };
        }

        string IInteractable.GetInteractionString()
        {
            return "Press F to interact";
        }

        void IInteractable.Interact(IInteractor interactor)
        {
            this.interactor = interactor;
            UISystem.GetUI<ShopUI>().Initialize(this);
            UISystem.Show<ShopUI>();
            UIEventSystem.Register<ShopUI>(this);
        }

        bool IInteractable.IsAvailableForInteraction()
        {
            return IsInInteraction == false;
        }

        Inventory IShopAnchor.GetInventory() => shopKeeperInventory;

        bool IShopAnchor.SellItem(int itemIndex)
        {
            ReadOnlyInventoryItem readOnlyInventoryItem = shopKeeperInventory[itemIndex];
            if (interactorInventory.CanAdd(readOnlyInventoryItem.Item, 1) == false)
            {
                Debug.LogWarning("Can't add this item");
                return false;
            }

            int removeAmount = 1;

            if (shopKeeperInventory.CanRemove(readOnlyInventoryItem.Item, removeAmount) == false)
            {
                Debug.LogWarning("Can't remove this item");
                return false;
            }

            // TODO : Define a price for items
            int coinCount = 0;
            var coinItems = interactorInventory.GetItemsOfType<CoinItem>((_) => true);
            int coinItemCount = coinItems.Count;
            for (int i = 0; i < coinItemCount; i++)
            {
                coinCount += coinItems[i].Amount;
            }

            // Lets say every item worths 10 coin
            int itemPrice = 10;
            if (coinCount - itemPrice < 0)
            {
                Debug.LogWarning("Not enough coin to buy" + readOnlyInventoryItem.Item.GetType().Name.Split('.')[^1]);
                return false;
            }

            // Remove coins
            interactorInventory.Remove(interactorInventory[itemIndex].Item, ref itemPrice);

            int addAmount = 1;
            interactorInventory.TryAdd(readOnlyInventoryItem.Item, ref addAmount);
            shopKeeperInventory.Remove(readOnlyInventoryItem.Item, ref removeAmount);
            return true;
        }

        void IUIEventListener.OnShowUI(GameUI ui)
        {
        }

        void IUIEventListener.OnHideUI(GameUI ui)
        {
            interactor.OnInteractionEnd(this);
        }
    }
}
