using System.Buffers;
using TheGame.Assets.Scripts.InteractionSystems;
using TheGame.FSM;
using UnityEngine;
using XIV.Core.Extensions;

namespace TheGame.PlayerSystems.States
{
    public class PlayerCheckInteractableState : State<PlayerFSM, PlayerStateFactory>
    {
        public IInteractable interactable { get; private set; }

        public PlayerCheckInteractableState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {

        }

        protected override void OnStateUpdate()
        {
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int hitCount = Physics2D.OverlapCircleNonAlloc(stateMachine.transform.position, 1f, buffer, 1 << PhysicsConstants.InteractableLayer);
            ArrayPool<Collider2D>.Shared.Return(buffer);

            if (hitCount == 0)
            {
                interactable = default;
                return;
            }

            interactable = buffer.GetClosestCollider(transform.position, hitCount).GetComponent<IInteractable>();
        }
    }
}