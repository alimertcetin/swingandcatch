using UnityEngine;

namespace TheGame.ScriptableObjects.StateDatas.SawBladeStateDatas
{
    [CreateAssetMenu(menuName = MenuPaths.SAW_BLADE_FSM_STATE_DATA + nameof(SawBladeTransitionToIdleStateDataSO))]
    public class SawBladeTransitionToIdleStateDataSO : StateDataSO
    {
        public float goToStartPositionSpeed = 5f;
    }
}