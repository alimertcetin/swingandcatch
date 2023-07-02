using UnityEngine;

namespace TheGame.ScriptableObjects.StateDatas.PlayerStateDatas
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM + nameof(PlayerStateDataCollectionSO))]
    public class PlayerStateDataCollectionSO : ScriptableObject
    {
        public PlayerWalkStateDataSO walkStateDataSO;
        public PlayerRunStateDataSO runStateDataSO;
        public PlayerAirMovementStateDataSO airMovementStateDataSO;
        public PlayerJumpStateDataSO jumpStateDataSO;
        public PlayerGroundedStateDataSO groundedStateDataSO;
        public PlayerFallStateDataSO fallStateDataSO;
        public PlayerClimbStateDataSO climbStateDataSO;
        public PlayerAttackStateDataSO attackStateDataSO;
    }
}