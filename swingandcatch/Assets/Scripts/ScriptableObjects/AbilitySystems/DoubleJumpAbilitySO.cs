using TheGame.AbilitySystems.Abilities;
using TheGame.AbilitySystems.Core;
using UnityEngine;

namespace TheGame.ScriptableObjects.AbilitySystems
{
    [CreateAssetMenu(menuName = MenuPaths.ABILITY_MENU + nameof(DoubleJumpAbilitySO))]
    public class DoubleJumpAbilitySO : AbilitySO<DoubleJumpAbility>
    {
        public override IAbility GetAbility() => new DoubleJumpAbility(abiility);
    }
}