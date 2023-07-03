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
        bool hasTarget;
        Collider2D currentTarget;
        
        public PlayerAttackState(PlayerFSM stateMachine, PlayerStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
            InputManager.Inputs.PlayerAttack.SetCallbacks(this);
        }

        protected override void OnStateEnter(State comingFrom) => InputManager.Inputs.PlayerAttack.Enable();

        protected override void OnStateUpdate()
        {
            void PlayAttackAnimationIfPossible(Collider2D coll)
            {
                if (CanPlayAttackAnimation())
                {
                    PlayAttackAnimation(GetSwingDirection(coll));
                }
            }
            
            if (TrySelectTarget(out var coll, out var damageable, out var distance) == false)
            {
                PlayAttackAnimationIfPossible(coll);
                DeselectTargetIfExists();
                return;
            }
            
            if (distance > stateMachine.stateDatas.attackStateDataSO.targetSelectionRadius)
            {
                PlayAttackAnimationIfPossible(coll);
                return;
            }

            // Handle selection
            if (coll != currentTarget) DeselectTargetIfExists();
            SetNewTarget(coll);

            // Can't attack right now
            if (distance > stateMachine.stateDatas.attackStateDataSO.attackRadius)
            {
                PlayAttackAnimationIfPossible(coll);
                return;
            }

            if (CanPlayAttackAnimation())
            {
                PlayAttackAnimation(GetSwingDirection(coll));
                damageable.ReceiveDamage(stateMachine.stateDatas.attackStateDataSO.damage);
                stateMachine.cameraShakeChannel.RaiseEvent(5f);
            }
        }

        protected override void OnStateExit() => InputManager.Inputs.PlayerAttack.Disable();

        public void OnAttack(InputAction.CallbackContext context)
        {
            attackPressed = context.performed;
        }

        bool TrySelectTarget(out Collider2D coll, out IDamageable damageable, out float distance)
        {
            coll = default;
            damageable = default;
            distance = default;
            
            var stateData = stateMachine.stateDatas.attackStateDataSO;
            
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            var pos = (Vector2)stateMachine.transform.position;
            int hitCount = Physics2D.OverlapCircleNonAlloc(pos, stateData.targetSelectionRadius, buffer, 1 << PhysicsConstants.EnemyLayer);
            if (hitCount == 0)
            {
                ArrayPool<Collider2D>.Shared.Return(buffer);
                return false;
            }

            coll = buffer.GetClosestCollider(pos, hitCount, out var positionOnCollider);
            distance = Vector2.Distance(positionOnCollider, pos);
            
            var dir = (positionOnCollider - pos).normalized;
            var raycastHitBuffer = ArrayPool<RaycastHit2D>.Shared.Rent(2);
            // There is an obstacle between player and the target - Skip
            if (Physics2D.LinecastNonAlloc(pos, pos + dir.normalized * stateData.attackRadius, raycastHitBuffer, 1 << PhysicsConstants.GroundLayer) > 0)
            {
                ArrayPool<Collider2D>.Shared.Return(buffer);
                ArrayPool<RaycastHit2D>.Shared.Return(raycastHitBuffer);
                return false;
            }

            // Target is not an IDamageable or It cant receive damage - Skip
            if (coll.TryGetComponent(out damageable) == false || damageable.CanReceiveDamage() == false)
            {
                ArrayPool<Collider2D>.Shared.Return(buffer);
                ArrayPool<RaycastHit2D>.Shared.Return(raycastHitBuffer);
                return false;
            }
            
            ArrayPool<Collider2D>.Shared.Return(buffer);
            ArrayPool<RaycastHit2D>.Shared.Return(raycastHitBuffer);
            return true;
        }
        
        Vector3 GetSwingDirection(Collider2D coll)
        {
            if (coll == default)
            {
                var velocity = stateMachine.velocity;
                var swingDir = velocity.normalized;
                if (velocity.sqrMagnitude - Mathf.Epsilon < Mathf.Epsilon)
                {
                    var angle = Random.value * 90f * (Random.value > 0.5f ? 1f : -1f);
                    swingDir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up;
                }

                swingDir.z = 0f;
                return swingDir;
            }
            
            var pos = stateMachine.transform.position;
            var dir = ((Vector3)coll.ClosestPoint(pos) - pos).normalized;
            return dir;
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

        bool CanPlayAttackAnimation()
        {
            return attackPressed && stateMachine.playerSword.HasTween() == false;
        }

        void DeselectTargetIfExists()
        {
            if (hasTarget) stateMachine.selectableDeselectChannel.RaiseEvent(currentTarget.transform);
            hasTarget = false;
            currentTarget = default;
        }

        void SetNewTarget(Collider2D target)
        {
            hasTarget = target;
            currentTarget = target;
            stateMachine.selectableSelectChannel.RaiseEvent(currentTarget.transform);
        }

    }
}