using System.Buffers;
using TheGame.FSM;
using TheGame.HealthSystems;
using TheGame.Scripts.InputSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using XIV.Core.Extensions;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;

namespace TheGame.PlayerSystems.States
{
    public class PlayerAttackState : State<PlayerFSM, PlayerStateFactory>, DefaultGameInputs.IPlayerAttackActions
    {
        bool attackPressed;
        
        public PlayerAttackState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
            InputManager.Inputs.PlayerAttack.SetCallbacks(this);
        }

        protected override void OnStateEnter(State comingFrom) => InputManager.Inputs.PlayerAttack.Enable();

        protected override void OnStateUpdate()
        {
            if (attackPressed == false) return;
            var stateData = stateMachine.stateDatas.attackStateDataSO;
            var radius = stateData.attackRadius;
            var sword = stateMachine.playerSword;
            
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            var pos = stateMachine.transform.position;
            int hitCount = Physics2D.OverlapCircleNonAlloc(pos, radius, buffer, 1 << PhysicsConstants.EnemyLayer);

            void SwingAndReturn()
            {
                var velocity = stateMachine.velocity;
                var swingDir = velocity.normalized;
                if (velocity.sqrMagnitude - Mathf.Epsilon < Mathf.Epsilon)
                {
                    var angle = Random.value * 90f * (Random.value > 0.5f ? 1f : -1f);
                    swingDir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up;
                }

                swingDir.z = 0f;
                
                SwingSword(sword, swingDir);
                ArrayPool<Collider2D>.Shared.Return(buffer);
            }

            if (hitCount == 0)
            {
                SwingAndReturn();
                return;
            }

            var coll = buffer.GetClosest(pos, hitCount);
            var dir = ((Vector3)coll.ClosestPoint(pos) - pos).normalized;
            var raycastHitBuffer = ArrayPool<RaycastHit2D>.Shared.Rent(2);
            if (Physics2D.LinecastNonAlloc(pos, pos + dir.normalized * radius, raycastHitBuffer, 1 << PhysicsConstants.GroundLayer) > 0)
            {
                SwingAndReturn();
                ArrayPool<RaycastHit2D>.Shared.Return(raycastHitBuffer);
                return;
            }

            if (coll.TryGetComponent(out IDamageable damageable) == false || damageable.CanReceiveDamage() == false)
            {
                SwingAndReturn();
                return;
            }

            if (SwingSword(sword, dir))
            {
                damageable.ReceiveDamage(stateData.damage);
            }

            ArrayPool<Collider2D>.Shared.Return(buffer);
        }

        protected override void OnStateExit() => InputManager.Inputs.PlayerAttack.Disable();

        public void OnAttack(InputAction.CallbackContext context)
        {
            attackPressed = context.performed;
        }

        static bool SwingSword(Transform sword, Vector3 direction)
        {
            if (sword.HasTween()) return false;

            var initialRotation = sword.transform.rotation;
            var lookRotation = Quaternion.LookRotation(direction);
            
            var rotationStart = lookRotation * Quaternion.AngleAxis(-30f, Vector3.right);
            var rotationEnd = rotationStart * Quaternion.AngleAxis(30f, Vector3.right);
            
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

            return true;
        }
    }
}