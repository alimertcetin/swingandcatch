using UnityEngine;

namespace TheGame.ScriptableObjects.StateDatas.PlayerStateDatas
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM_STATE_DATA + nameof(PlayerAirMovementStateDataSO))]
    public class PlayerAirMovementStateDataSO : StateDataSO
    {
        public float airMovementSpeed = 7.5f;
    }
}