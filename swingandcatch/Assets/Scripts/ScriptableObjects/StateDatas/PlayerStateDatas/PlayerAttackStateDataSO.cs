using UnityEngine;

namespace TheGame.ScriptableObjects.StateDatas.PlayerStateDatas
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM_STATE_DATA + nameof(PlayerAttackStateDataSO))]
    public class PlayerAttackStateDataSO : StateDataSO
    {
        public float targetSelectionRadius = 4f;
        public float attackRadius;
        public float damage;
    }
}