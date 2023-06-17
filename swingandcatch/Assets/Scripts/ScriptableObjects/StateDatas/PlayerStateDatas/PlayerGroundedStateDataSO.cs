using TheGame.ScriptableObjects;
using TheGame.ScriptableObjects.StateDatas;
using UnityEngine;

namespace TheGame.PlayerSystems
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM_STATE_DATA + nameof(PlayerGroundedStateDataSO))]
    public class PlayerGroundedStateDataSO : StateDataSO
    {
        public float groundCheckDistance = 0.15f;
    }
}