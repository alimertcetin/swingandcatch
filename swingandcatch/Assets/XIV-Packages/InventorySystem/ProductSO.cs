using UnityEngine;
using XIV.InventorySystem.ScriptableObjects;

namespace TheGame.InteractionSystems
{
    [CreateAssetMenu(menuName = MenuPaths.ITEMS_NONSERIALIZED_DATA_MENU + nameof(ProductSO))]
    public class ProductSO : ScriptableObject
    {
        public ItemSO itemSO;
        public int price;
    }
}