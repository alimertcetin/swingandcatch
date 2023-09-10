using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.UI;
using XIV.InventorySystem;
using XIV.InventorySystem.ScriptableObjects.NonSerializedData;

namespace TheGame.UISystems.Shop
{
    public class ShopUI : GameUI, IShopUIItemListener
    {
        [SerializeField] NonSerializedItemDataContainerSO nonSerializedItemDataContainerSO;
        [SerializeField] ShopUIItem shopUIItemPrefab;
        [SerializeField] RectTransform contentParent;

        // TODO : Escape to quit
        [SerializeField] Button btn_Exit;

        IShopAnchor shopAnchor;

        public void Initialize(IShopAnchor shopAnchor)
        {
            this.shopAnchor = shopAnchor;
        }

        void OnEnable()
        {
            btn_Exit.onClick.AddListener(OnExitClicked);
        }

        void OnDisable()
        {
            btn_Exit.onClick.RemoveListener(OnExitClicked);
        }

        void OnExitClicked()
        {
            UISystem.Hide<ShopUI>();
        }

        public override void Show()
        {
            base.Show();
            var inventory = shopAnchor.GetInventory();
            int slotCount = inventory.SlotCount;
            for (int i = 0; i < slotCount; i++)
            {
                if (inventory[i].IsEmpty) continue;

                InitializeShopUIItem(i, inventory, Instantiate(shopUIItemPrefab, contentParent));
            }
        }

        protected override void OnUIDeactivated()
        {
            int childCount = contentParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(contentParent.GetChild(i).gameObject);
            }
        }

        void IShopUIItemListener.OnBuy(int itemIndex)
        {
            if (shopAnchor.SellItem(itemIndex))
            {
                var shopUIItem = contentParent.GetChild(itemIndex).GetComponent<ShopUIItem>();
                InitializeShopUIItem(itemIndex, shopAnchor.GetInventory(), shopUIItem);
            }
        }

        void InitializeShopUIItem(int index, Inventory inventory, ShopUIItem shopUIItem)
        {
            ReadOnlyInventoryItem readonlyInventoryItem = inventory[index];
            int itemAmount = readonlyInventoryItem.Amount;
            Sprite itemSprite = nonSerializedItemDataContainerSO.GetSprite(readonlyInventoryItem.Item);
            shopUIItem.Initialize(index, itemAmount, itemSprite, this);
        }
    }
}
