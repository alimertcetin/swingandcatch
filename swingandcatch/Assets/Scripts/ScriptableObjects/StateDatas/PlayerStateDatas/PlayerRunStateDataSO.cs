using TheGame.ScriptableObjects;
using TheGame.ScriptableObjects.StateDatas;
using UnityEngine;

namespace TheGame.PlayerSystems
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM_STATE_DATA + nameof(PlayerRunStateDataSO))]
    public class PlayerRunStateDataSO : StateDataSO
    {
        public float runSpeed = 10f;
    }
}