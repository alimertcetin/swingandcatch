using XIV.InventorySystem;

namespace TheGame.UISystems.Shop
{
    public interface IShopAnchor
    {
        Inventory GetInventory();

        /// <summary>
        /// Returns <see langword="true"/> if sold, <see langword="false"/> otherwise
        /// </summary>
        bool SellItem(int itemIndex);
    }
}
