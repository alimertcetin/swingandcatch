using UnityEngine;

namespace TheGame.ScriptableObjects.StateDatas.PlayerStateDatas
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM_STATE_DATA + nameof(PlayerWalkStateDataSO))]
    public class PlayerWalkStateDataSO : StateDataSO
    {
        public float walkSpeed = 5f;
    }
}