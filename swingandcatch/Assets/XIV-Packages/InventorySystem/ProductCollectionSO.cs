using UnityEngine;
using XIV.Core;
using XIV.InventorySystem;
using XIV.InventorySystem.ScriptableObjects;

namespace TheGame.InteractionSystems
{
    [CreateAssetMenu(menuName = MenuPaths.ITEMS_NONSERIALIZED_DATA_MENU + nameof(ProductCollectionSO))]
    public class ProductCollectionSO : ScriptableObject
    {
        public ProductSO[] products;

        public int GetPrice(ItemBase itemBase)
        {
            int length = products.Length;
            for (int i = 0; i < length; i++)
            {
                if (products[i].itemSO.GetItem().Id == itemBase.Id)
                {
                    return products[i].price;
                }
            }

            return -1;
        }

#if UNITY_EDITOR

        [Button]
        void LoadProducts()
        {
            var dataContainers = XIV.XIVEditor.Utils.AssetUtils.LoadAssetsOfType<ProductSO>("Assets/ScriptableObjects");

            UnityEditor.Undo.RecordObject(this, "Load Products");
            products = new ProductSO[dataContainers.Count];
            for (int i = 0; i < dataContainers.Count; i++)
            {
                products[i] = dataContainers[i];
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif
    }
}