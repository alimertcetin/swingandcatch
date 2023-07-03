using System.Buffers;
using TheGame.HealthSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using XIV.Core.Extensions;

namespace TheGame.PlayerSystems.States.Helpers
{
    public class AttackTargetSelectionHandler
    {
        readonly PlayerFSM playerFSM;
        public Collider2D currentSelection;

        public AttackTargetSelectionHandler(PlayerFSM playerFSM)
        {
            this.playerFSM = playerFSM;
        }

        public void HandleSelection()
        {
            // Deselect the current selection if it fails validation
            if (ShouldDeselectCurrent())
            {
                DeselectCurrent();
            }
            else
            {
                if (currentSelection) playerFSM.selectableSelectChannel.RaiseEvent(currentSelection.transform);
            }

            var collider = GetSelectable();
            
            // Select the new collider if it passes the validation
            if (ShouldSelectCollider(collider))
            {
                SelectCollider(collider);
            }
        }

        bool ShouldDeselectCurrent()
        {
            return (currentSelection != null) && (IsColliderValid(currentSelection) == false || ValidateDistance(currentSelection) == false);
        }

        bool ShouldSelectCollider(Collider2D collider)
        {
            return (collider != null) && (currentSelection != collider) && ValidateDistance(collider) && IsColliderValid(collider) && ValidateIDamageable(collider);
        }
        
        bool IsColliderValid(Collider2D collider)
        {
            // Check for a wall between the player and collider
            // Return true if the collider is valid, false otherwise
            var pos = playerFSM.transform.position;
            var buffer = ArrayPool<RaycastHit2D>.Shared.Rent(2);
            var hitCount = Physics2D.LinecastNonAlloc(pos, collider.ClosestPoint(pos), buffer, 1 << PhysicsConstants.GroundLayer);
            ArrayPool<RaycastHit2D>.Shared.Return(buffer);
            return hitCount == 0;
        }

        bool ValidateDistance(Collider2D collider)
        {
            // Check if the collider is within the desired distance from the player
            // Return true if the collider is within distance, false otherwise
            var mouseWorldPosition = GetMouseWorldPosition();
            var closestPosOnCollider = collider.ClosestPoint(mouseWorldPosition);
            var distance = Vector3.Distance(mouseWorldPosition, closestPosOnCollider);
            return distance < playerFSM.stateDatas.attackStateDataSO.targetSelectionRadius;
        }

        bool ValidateIDamageable(Collider2D collider)
        {
            // Check if the collider has the IDamageable component
            // Return true if the collider has the component, false otherwise
            var hasComponent = collider.TryGetComponent(out IDamageable damageable);
            var canReceiveDamage = hasComponent && damageable.CanReceiveDamage();
            return hasComponent && canReceiveDamage;
        }

        void DeselectCurrent()
        {
            if (currentSelection != null) playerFSM.selectableDeselectChannel.RaiseEvent(currentSelection.transform);
            // Deselect the currentSelection
            currentSelection = null;
        }

        void SelectCollider(Collider2D collider)
        {
            // Select the new collider
            currentSelection = collider;
            if (currentSelection != null) playerFSM.selectableSelectChannel.RaiseEvent(currentSelection.transform);
        }

        Collider2D GetSelectable()
        {
            // Perform a raycast from the mouse cursor and return the collider hit
            // ...
            var mouseWorldPosition = GetMouseWorldPosition();

            var stateData = playerFSM.stateDatas.attackStateDataSO;
            var selectionDistance = stateData.targetSelectionRadius;

            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int hitCount = Physics2D.OverlapCircleNonAlloc(mouseWorldPosition, selectionDistance, buffer, 1 << PhysicsConstants.EnemyLayer);
            var closest = buffer.GetClosestCollider(mouseWorldPosition, hitCount, out var positionOnCollider);
            ArrayPool<Collider2D>.Shared.Return(buffer);

            return closest;
        }

        public static Vector3 GetMouseWorldPosition()
        {
            var cam = Camera.main;
            var mouseScreenPos = (Vector3)Mouse.current.position.ReadValue();
            var camPos = cam.transform.position;
            var mouseWorldPosition = cam.ScreenToWorldPoint(mouseScreenPos.SetZ(Mathf.Abs(camPos.z)));
            return mouseWorldPosition;
        }
    }
}