﻿using UnityEngine;

namespace TheGame.ScriptableObjects.StateDatas.PlayerStateDatas
{
    [CreateAssetMenu(menuName = MenuPaths.PLAYER_FSM_STATE_DATA + nameof(PlayerJumpStateDataSO))]
    public class PlayerJumpStateDataSO : StateDataSO
    {
        public float jumpHeight = 3f;
        public float jumpGravityScale = 1f;
    }
}