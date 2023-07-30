using TheGame.AbilitySystems;
using TheGame.AbilitySystems.Abilities;
using TheGame.FSM;
using TheGame.Scripts.InputSystems;
using UnityEngine.InputSystem;

namespace TheGame.PlayerSystems.States
{
    public class PlayerAttackState : State<PlayerFSM, PlayerStateFactory>, DefaultGameInputs.IPlayerAttackActions
    {
        bool attackPressed;
        
        public PlayerAttackState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
            InputManager.Inputs.PlayerAttack.SetCallbacks(this);
        }

        protected override void OnStateEnter(State comingFrom)
        {
            InputManager.Inputs.PlayerAttack.Enable();
        }

        protected override void OnStateUpdate()
        {
            if (attackPressed == false) return;
            var attackAbility = stateMachine.abilityHandler.GetAbility<AttackAbility>();
            if (attackAbility != null) stateMachine.abilityHandler.UseAbility(attackAbility);
        }

        protected override void OnStateExit()
        {
            InputManager.Inputs.PlayerAttack.Disable();
        }

        void DefaultGameInputs.IPlayerAttackActions.OnAttack(InputAction.CallbackContext context)
        {
            attackPressed = context.performed;
        }
    }
}