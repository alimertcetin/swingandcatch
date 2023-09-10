using UnityEngine;

namespace XIV.InventorySystem.ScriptableObjects.NonSerializedData
{
    [CreateAssetMenu(menuName = MenuPaths.ITEMS_NONSERIALIZED_DATA_MENU + nameof(NonSerializedItemDataContainerSO))]
    public class NonSerializedItemDataContainerSO : ScriptableObject
    {
        public NonSerializedItemDataSO[] itemDataPairs;

        public Sprite GetSprite(ItemBase itemBase)
        {
            for (int i = 0; i < itemDataPairs.Length; i++)
            {
                if (itemDataPairs[i].itemSO.GetItem().Id == itemBase.Id)
                {
                    return itemDataPairs[i].uiSprite;
                }
            }

            return null;
        }
    }
}