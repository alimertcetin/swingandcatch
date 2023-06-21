using UnityEngine;
using XIV.Core.Utils;

namespace TheGame.ScriptableObjects.StateDatas.SawBladeStateDatas
{
    [CreateAssetMenu(menuName = MenuPaths.SAW_BLADE_FSM_STATE_DATA + nameof(SawBladeIdleStateDataSO))]
    public class SawBladeIdleStateDataSO : StateDataSO
    {
        public Vector3 idleMovementAxis = Vector3.right;
        public float idleMovementDistance = 2f;
        public float idleMovementSpeed = 2f;
        public EasingFunction.Ease ease;
    }
}