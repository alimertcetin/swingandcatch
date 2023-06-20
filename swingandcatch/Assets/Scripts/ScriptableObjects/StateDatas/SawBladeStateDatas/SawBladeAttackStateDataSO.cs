using UnityEngine;

namespace TheGame.ScriptableObjects.StateDatas.SawBladeStateDatas
{
    [CreateAssetMenu(menuName = MenuPaths.SAW_BLADE_FSM_STATE_DATA + nameof(SawBladeAttackStateDataSO))]
    public class SawBladeAttackStateDataSO : StateDataSO
    {
        public float attackDuration = 1f;
        public float followDistance = 1.5f;
        public float followSpeed = 2;
        public float attackFieldRadius = 2f;
        public GameObject projectilePrefab;
    }
}