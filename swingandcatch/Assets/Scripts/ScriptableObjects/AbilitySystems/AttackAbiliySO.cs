using TheGame.AbilitySystems.Abilities;
using UnityEngine;
using XIV.InventorySystem.ScriptableObjects;

namespace TheGame.ScriptableObjects.AbilitySystems
{
    [CreateAssetMenu(menuName = MenuPaths.ABILITY_MENU + nameof(AttackAbiliySO))]
    public class AttackAbiliySO : ItemSO<AttackAbility>
    {
        
    }
}