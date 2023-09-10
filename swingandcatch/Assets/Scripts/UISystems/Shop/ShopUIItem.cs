using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UISystems.Shop
{
    public class ShopUIItem : MonoBehaviour
    {
        [SerializeField] Image img_Icon;
        [SerializeField] TMP_Text txt_Amount; // unlimited?
        [SerializeField] Button btn_Buy;

        int itemIndex;
        IShopUIItemListener shopUIItemListener;

        public void Initialize(int indexInInventory, int itemAmount, Sprite itemSprite, IShopUIItemListener shopUIItemListener)
        {
            this.shopUIItemListener = shopUIItemListener;
            itemIndex = indexInInventory;
            txt_Amount.text = itemAmount.ToString();
            img_Icon.sprite = itemSprite;
        }

        void OnEnable()
        {
            btn_Buy.onClick.AddListener(OnBuyClicked);
        }

        void OnDisable()
        {
            btn_Buy.onClick.RemoveListener(OnBuyClicked);
        }

        void OnBuyClicked()
        {
            shopUIItemListener.OnBuy(itemIndex);
        }
    }
}
