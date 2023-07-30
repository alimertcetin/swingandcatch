using UnityEngine;

namespace TheGame.Interfaces
{
    public interface IRotationHandler
    {
        bool Rotate(Quaternion newRotation);
        bool LookDirection(Vector3 direction);
    }
}