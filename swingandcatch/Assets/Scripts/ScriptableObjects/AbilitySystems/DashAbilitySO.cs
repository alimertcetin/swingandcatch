using TheGame.AbilitySystems.Abilities;
using TheGame.AbilitySystems.Core;
using UnityEngine;

namespace TheGame.ScriptableObjects.AbilitySystems
{
    [CreateAssetMenu(menuName = MenuPaths.ABILITY_MENU + nameof(DashAbilitySO))]
    public class DashAbilitySO : AbilitySO<DashAbility>
    {
        public override IAbility GetAbility()
        {
            return new DashAbility(abiility);
        }
    }
}