using TheGame.FSM;
using TheGame.HealthSystems;
using TheGame.PlayerSystems.States.Helpers;
using TheGame.Scripts.InputSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;

namespace TheGame.PlayerSystems.States
{
    public class PlayerAttackState : State<PlayerFSM, PlayerStateFactory>, DefaultGameInputs.IPlayerAttackActions
    {
        bool attackPressed;
        AttackTargetSelectionHandler attackTargetSelectionHandler;
        
        public PlayerAttackState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
            InputManager.Inputs.PlayerAttack.SetCallbacks(this);
            attackTargetSelectionHandler = new AttackTargetSelectionHandler(stateMachine);
        }

        protected override void OnStateEnter(State comingFrom) => InputManager.Inputs.PlayerAttack.Enable();

        protected override void OnStateUpdate()
        {
            attackTargetSelectionHandler.HandleSelection();
            if (attackPressed == false || stateMachine.playerSword.HasTween()) return;
            
            var selectedCollider = attackTargetSelectionHandler.currentSelection;
            PlayAttackAnimation(GetSwingDirection(selectedCollider));
                    
            if (selectedCollider != default)
            {
                var stateMachineTransformPosition = stateMachine.transform.position;
                var closestPointOnCollider = (Vector3)selectedCollider.ClosestPoint(stateMachineTransformPosition);
                var distance = Vector3.Distance(stateMachineTransformPosition, closestPointOnCollider);
                var stateData = stateMachine.stateDatas.attackStateDataSO;
                if (distance < stateData.attackRadius)
                {
                    selectedCollider.GetComponent<IDamageable>().ReceiveDamage(stateData.damage);
                }
            }

        }

        protected override void OnStateExit() => InputManager.Inputs.PlayerAttack.Disable();

        public void OnAttack(InputAction.CallbackContext context)
        {
            attackPressed = context.performed;
        }
        
        Vector3 GetSwingDirection(Collider2D coll)
        {
            Vector3 startPoint;
            Vector3 targetPoint;
            if (coll != default)
            {
                startPoint = stateMachine.transform.position;
                targetPoint = coll.ClosestPoint(startPoint);
            }
            else
            {
                startPoint = stateMachine.transform.position;
                targetPoint = AttackTargetSelectionHandler.GetMouseWorldPosition();
            }

            return (targetPoint - startPoint).normalized;
        }

        void PlayAttackAnimation(Vector3 swingDirection)
        {
            var sword = stateMachine.playerSword;
            var initialRotation = sword.transform.rotation;
            var lookRotation = Quaternion.LookRotation(swingDirection);
            
            var rotationStart = lookRotation * Quaternion.AngleAxis(45f, Vector3.left);
            var rotationEnd = rotationStart * Quaternion.AngleAxis(90f, Vector3.right);
            
            sword.transform.rotation = rotationStart;

            sword.gameObject.SetActive(true);
            sword.XIVTween()
                .Rotate(rotationStart, rotationEnd, 0.25f, EasingFunction.EaseInOutExpo)
                .OnComplete(() =>
                {
                    sword.gameObject.SetActive(false);
                    sword.rotation = initialRotation;
                })
                .Start();
        }
        
    }
}