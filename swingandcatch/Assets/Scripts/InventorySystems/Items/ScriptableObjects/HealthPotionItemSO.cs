using UnityEngine;
using XIV.InventorySystem.ScriptableObjects;

namespace TheGame.InventorySystems.Items.ScriptableObjects
{
    [CreateAssetMenu(menuName = MenuPaths.ITEMS_MENU + nameof(HealthPotionItemSO))]
    public class HealthPotionItemSO : ItemSO<HealthPotionItem>
    {

    }
}