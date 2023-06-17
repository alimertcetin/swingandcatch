using TheGame.ScriptableObjects;
using TheGame.ScriptableObjects.StateDatas;
using UnityEngine;

namespace TheGame.PlayerSystems
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM_STATE_DATA + nameof(PlayerWalkStateDataSO))]
    public class PlayerWalkStateDataSO : StateDataSO
    {
        public float walkSpeed = 5f;
    }
}