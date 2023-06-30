using UnityEngine;

namespace TheGame.ScriptableObjects.StateDatas.PlayerStateDatas
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM_STATE_DATA + nameof(PlayerFallStateDataSO))]
    public class PlayerFallStateDataSO : StateDataSO
    {
        public float fallGravityScale = 1f;
    }
}