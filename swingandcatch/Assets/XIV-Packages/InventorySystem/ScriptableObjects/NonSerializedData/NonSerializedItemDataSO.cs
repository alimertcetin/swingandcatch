using UnityEngine;

namespace XIV.InventorySystem.ScriptableObjects.NonSerializedData
{
    [CreateAssetMenu(menuName = MenuPaths.ITEMS_NONSERIALIZED_DATA_MENU + nameof(NonSerializedItemDataSO))]
    public class NonSerializedItemDataSO : ScriptableObject
    {
        public ItemSO itemSO;
        public Sprite uiSprite;
    }
}