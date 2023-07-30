using UnityEngine;

namespace TheGame.Interfaces
{
    public interface IMovementHandler
    {
        void SyncPosition();
        Vector3 GetPreviousPosition();
        Vector3 GetCurrentVelocity();
        bool Move(Vector3 targetPosition);
        bool CanMove(Vector3 targetPosition);
        bool CheckIsTouching(int layerMask);
        int CheckIsTouchingNonAlloc(Collider2D[] buffer, int layerMask);
    }
}