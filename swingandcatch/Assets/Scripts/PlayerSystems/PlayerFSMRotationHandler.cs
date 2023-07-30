using TheGame.Interfaces;
using UnityEngine;

namespace TheGame.PlayerSystems
{
    public class PlayerFSMRotationHandler : MonoBehaviour, IRotationHandler
    {
        public bool Rotate(Quaternion newRotation)
        {
            transform.rotation = newRotation;
            return true;
        }

        public bool LookDirection(Vector3 direction)
        {
            direction.Normalize();
            return direction.sqrMagnitude > Mathf.Epsilon && Rotate(Quaternion.LookRotation(direction));
        }
    }
}