using TheGame.Assets.Scripts.InteractionSystems;
using TheGame.FSM;

namespace TheGame.PlayerSystems.States
{
    public class PlayerInteractionState : State<PlayerFSM, PlayerStateFactory>, IInteractor
    {
        bool interactionEnded;

        public PlayerInteractionState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {

        }

        protected override void CheckTransitions()
        {
            if (interactionEnded)
            {
                interactionEnded = false;
                ChangeRootState(factory.GetState<PlayerGroundedState>());
                return;
            }
        }

        void IInteractor.OnInteractionEnd(IInteractable interactable)
        {
            interactionEnded = true;
        }
    }
}