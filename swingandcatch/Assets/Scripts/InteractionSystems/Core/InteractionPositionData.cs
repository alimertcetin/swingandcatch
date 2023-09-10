using UnityEngine;

namespace TheGame.Assets.Scripts.InteractionSystems
{
    public struct InteractionPositionData
    {
        public Vector3 startPos; // Start position of interactor
        public Vector3 targetPosition; // target position of interactable in order to be able to interact with the object
        public Vector3 targetForwardDirection;
    }
}
