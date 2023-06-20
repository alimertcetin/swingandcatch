using System.Buffers;
using TheGame.FSM;
using TheGame.HazzardSystems;
using UnityEngine;
using UnityEngine.Pool;
using XIV.Core.Extensions;
using XIV.Core.Utils;
using Object = UnityEngine.Object;

namespace TheGame.EnemySystems.SawBlade.States
{
    public class SawBladeAttackState : State<SawBladeFSM, SawBladeStateFactory>
    {
        public Transform target;
        public Vector3 connectedPoint;
        Timer attackTimer;
        ObjectPool<GameObject> projectilePool;
        Collider2D[] buffer;
        
        public SawBladeAttackState(SawBladeFSM stateMachine, SawBladeStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
            projectilePool = new ObjectPool<GameObject>(() => Object.Instantiate(this.stateMachine.attackStateDataSO.projectilePrefab), 
                (go) => go.SetActive(true), 
                (go) => go.SetActive(false));
        }
        
        ~SawBladeAttackState() => projectilePool.Dispose();

        protected override void OnStateEnter(State comingFrom)
        {
            attackTimer = new Timer(stateMachine.attackStateDataSO.attackDuration);
            buffer = ArrayPool<Collider2D>.Shared.Rent(2);
        }

        protected override void OnStateUpdate()
        {
            HandleMovement();
            attackTimer.Update(Time.deltaTime);
            if (attackTimer.IsDone)
            {
                attackTimer.Restart();
                var projectile = projectilePool.Get().GetComponent<SawBladeProjectile>();
                var stateMachineTransformPosition = stateMachine.transform.position;
                projectile.transform.position = stateMachineTransformPosition;
                projectile.target = target;
                projectile.direction = (target.position - stateMachineTransformPosition).normalized.SetZ(0f);
                
                var hazzarMono = projectile.GetComponent<HazzardMono>();
                hazzarMono.RegisterHit(OnPlayerHit);

                projectile.onOutsideOfTheView = () =>
                {
                    hazzarMono.UnregisterHit(OnPlayerHit);
                    projectilePool.Release(projectile.gameObject);
                };
                void OnPlayerHit(Transform player)
                {
                    // TODO : Create particle pool
                    hazzarMono.UnregisterHit(OnPlayerHit);
                    var particleGo = Object.Instantiate(projectile.particlePrefab, projectile.transform.position, Quaternion.identity);
                    Object.Destroy(particleGo, 7f);
                    projectilePool.Release(projectile.gameObject);
                }
            }
        }

        protected override void OnStateExit()
        {
            ArrayPool<Collider2D>.Shared.Return(buffer);
        }

        protected override void CheckTransitions()
        {
            var attackStateData = stateMachine.attackStateDataSO;
            var center = Vector3.Lerp(stateMachine.idleStartPosition, stateMachine.idleEndPosition, 0.5f);
            int count = Physics2D.OverlapCircleNonAlloc(center, attackStateData.attackFieldRadius, buffer, 1 << PhysicsConstants.PlayerLayer);

            if (count == 0)
            {
                ChangeRootState(factory.GetState<SawBladeIdleTransitionState>());
                return;
            }
        }

        void HandleMovement()
        {
            var attackStateData = stateMachine.attackStateDataSO;
            var dir = target.position - connectedPoint;
            var distance = dir.magnitude;
            if (distance > attackStateData.followDistance)
            {
                var pos = stateMachine.transform.position;
                var targetPos = target.position - dir.normalized * attackStateData.followDistance;
                var newPos = Vector3.MoveTowards(pos, targetPos, attackStateData.followSpeed * Time.deltaTime);
                stateMachine.transform.position = newPos;
            }
        }
    }
}
