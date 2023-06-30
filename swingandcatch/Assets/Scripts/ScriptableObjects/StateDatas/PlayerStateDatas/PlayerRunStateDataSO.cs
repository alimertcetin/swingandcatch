using UnityEngine;

namespace TheGame.ScriptableObjects.StateDatas.PlayerStateDatas
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM_STATE_DATA + nameof(PlayerRunStateDataSO))]
    public class PlayerRunStateDataSO : StateDataSO
    {
        public float runSpeed = 10f;
    }
}