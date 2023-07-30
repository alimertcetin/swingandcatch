using TheGame.AbilitySystems;
using TheGame.AbilitySystems.Core;
using TheGame.HealthSystems;
using TheGame.Interfaces;
using TheGame.PlayerSystems.States.Helpers;
using UnityEngine;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;

namespace TheGame.PlayerSystems.States
{
    public class PlayerFSMAttackHandler : MonoBehaviour, IAttackHandler
    {
        PlayerFSM playerFsm;
        AttackTargetSelectionHandler attackTargetSelectionHandler;

        void Awake()
        {
            playerFsm = GetComponent<PlayerFSM>();
            attackTargetSelectionHandler = new AttackTargetSelectionHandler(playerFsm);
        }

        void Update()
        {
            attackTargetSelectionHandler.HandleSelection();
        }

        bool IAttackHandler.CanAttack()
        {
            return playerFsm.playerSword.HasTween() == false;
        }

        void IAttackHandler.Attack()
        {
            var selectedCollider = attackTargetSelectionHandler.currentSelection;
            PlayAttackAnimation(GetSwingDirection(selectedCollider));
                    
            if (selectedCollider != default)
            {
                var stateMachineTransformPosition = transform.position;
                var closestPointOnCollider = (Vector3)selectedCollider.ClosestPoint(stateMachineTransformPosition);
                var distance = Vector3.Distance(stateMachineTransformPosition, closestPointOnCollider);
                var stateData = playerFsm.stateDatas.attackStateDataSO;
                if (distance < stateData.attackRadius)
                {
                    var damageable = selectedCollider.GetComponent<IDamageable>();
                    if (damageable.CanReceiveDamage()) damageable.ReceiveDamage(stateData.damage);
                }
            }
        }
        
        Vector3 GetSwingDirection(Collider2D coll)
        {
            Vector3 startPoint;
            Vector3 targetPoint;
            if (coll != default)
            {
                startPoint = transform.position;
                targetPoint = coll.ClosestPoint(startPoint);
            }
            else
            {
                startPoint = transform.position;
                targetPoint = AttackTargetSelectionHandler.GetMouseWorldPosition();
            }

            return (targetPoint - startPoint).normalized;
        }

        void PlayAttackAnimation(Vector3 swingDirection)
        {
            var sword = playerFsm.playerSword;
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