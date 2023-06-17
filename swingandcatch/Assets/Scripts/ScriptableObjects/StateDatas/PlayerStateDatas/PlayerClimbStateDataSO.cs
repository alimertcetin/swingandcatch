using TheGame.ScriptableObjects;
using TheGame.ScriptableObjects.StateDatas;
using UnityEngine;

namespace TheGame.PlayerSystems
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM_STATE_DATA + nameof(PlayerClimbStateDataSO))]
    public class PlayerClimbStateDataSO : StateDataSO
    {
        public float climbCheckRadius = 1.1f;
        public float climbSpeed = 1f;
        public float ropeSwingForce = 2f;
        public float ropeSwingInitialForce = 120f;
    }
}