using TheGame.AbilitySystems.Abilities;
using TheGame.AbilitySystems.Core;
using UnityEngine;

namespace TheGame.ScriptableObjects.AbilitySystems
{
    [CreateAssetMenu(menuName = MenuPaths.ABILITY_MENU + nameof(AttackAbiliySO))]
    public class AttackAbiliySO : AbilitySO<AttackAbility>
    {
        public override IAbility GetAbility() => new AttackAbility();
    }
}