using TheGame.ScriptableObjects;
using TheGame.ScriptableObjects.StateDatas;
using UnityEngine;

namespace TheGame.PlayerSystems
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM_STATE_DATA + nameof(PlayerFallStateDataSO))]
    public class PlayerFallStateDataSO : StateDataSO
    {
        public float fallGravityScale = 1f;
    }
}