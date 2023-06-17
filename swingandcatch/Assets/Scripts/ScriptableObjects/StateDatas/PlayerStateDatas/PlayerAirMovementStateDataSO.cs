using TheGame.ScriptableObjects;
using TheGame.ScriptableObjects.StateDatas;
using UnityEngine;

namespace TheGame.PlayerSystems
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM_STATE_DATA + nameof(PlayerAirMovementStateDataSO))]
    public class PlayerAirMovementStateDataSO : StateDataSO
    {
        public float airMovementSpeed = 7.5f;
    }
}