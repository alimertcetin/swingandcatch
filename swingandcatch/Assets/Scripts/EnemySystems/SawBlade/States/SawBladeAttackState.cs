using System.Buffers;
using TheGame.FSM;
using TheGame.HazzardSystems;
using TheGame.HealthSystems;
using UnityEngine;
using UnityEngine.Pool;
using XIV.Core.Extensions;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.EventSystem;
using XIV.EventSystem.Events;
using Object = UnityEngine.Object;

namespace TheGame.EnemySystems.SawBlade.States
{
    public class SawBladeAttackState : State<SawBladeFSM, SawBladeStateFactory>
    {
        public Transform target;
        public Vector3 connectedPoint;
        Timer attackTimer;
        ObjectPool<Projectile> projectilePool;
        ObjectPool<GameObject> particlePool;
        
        public SawBladeAttackState(SawBladeFSM stateMachine, SawBladeStateFactory stateFactory) : base(stateMachine, stateFactory)
        {
            projectilePool = new ObjectPool<Projectile>(OnCreateProjectile, null, OnReleaseProjectile);
            particlePool = new ObjectPool<GameObject>(OnCreateParticle, null, OnReleaseParticle);
        }

        protected override void OnStateEnter(State comingFrom)
        {
            attackTimer = new Timer(stateMachine.attackStateDataSO.attackDuration);
        }

        protected override void OnStateUpdate()
        {
            HandleMovement();

            var bounds = stateMachine.collider2D.bounds;
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int hitCount = Physics2D.OverlapBoxNonAlloc(stateMachine.transform.position, bounds.size, 0f, buffer, 1 << PhysicsConstants.PlayerLayer);

            for (int i = 0; i < hitCount; i++)
            {
                var coll = buffer[i];
                if (coll.TryGetComponent(out IDamageable damageable))
                {
                    if (damageable.CanReceiveDamage()) damageable.ReceiveDamage(stateMachine.attackStateDataSO.collisionDamage);
                }
            }

            if (hitCount > 0)
            {
                if (stateMachine.transform.HasTween()) return;
                stateMachine.transform.XIVTween()
                    .Scale(Vector3.one, Vector3.one * 0.75f, 0.5f, EasingFunction.EaseInOutBounce, true, 1)
                    .And()
                    .RotateZ(0, 360f, 0.5f, EasingFunction.SmoothStop4)
                    .Start();
                var renderer = stateMachine.GetComponentInChildren<Renderer>();
                renderer.CancelTween();
                renderer.XIVTween()
                    .RendererColor(renderer.material.color, Color.white, 0.25f, EasingFunction.EaseInOutBounce, true, 1)
                    .Start();
            }
            
            ArrayPool<Collider2D>.Shared.Return(buffer);
            
            if (attackTimer.Update(Time.deltaTime))
            {
                attackTimer.Restart();
                LaunchProjectile();
            }
        }

        protected override void CheckTransitions()
        {
            var attackStateData = stateMachine.attackStateDataSO;
            var center = Vector3.Lerp(stateMachine.idleStartPosition, stateMachine.idleEndPosition, 0.5f);
            
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            int count = Physics2D.OverlapCircleNonAlloc(center, attackStateData.attackFieldRadius, buffer, 1 << PhysicsConstants.PlayerLayer);
            ArrayPool<Collider2D>.Shared.Return(buffer);

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

        void LaunchProjectile()
        {
            var projectile = projectilePool.Get();
            var layerMask = (1 << PhysicsConstants.PlayerLayer) | (1 << PhysicsConstants.GroundLayer) | (1 << PhysicsConstants.LavaLayer);
            var position = stateMachine.transform.position;
            var moveDirection = (target.position - position).normalized.SetZ(0f);
            var attackStateData = stateMachine.attackStateDataSO;
            var lifeTime = attackStateData.projectileLifeTime;
            var speed = attackStateData.projectileSpeed;
            var projectileDamage = attackStateData.projectileDamageAmount;
            projectile.Initialize(projectilePool, lifeTime, speed, projectileDamage, moveDirection, layerMask);
            projectile.transform.position = position;
            var trailRenderer = projectile.GetComponentInChildren<TrailRenderer>();
            trailRenderer.Clear();
            projectile.gameObject.SetActive(true);
        }
        
        Projectile OnCreateProjectile()
        {
            return Object.Instantiate(this.stateMachine.attackStateDataSO.projectilePrefab);
        }

        void OnReleaseProjectile(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
            var particleGo = particlePool.Get();
            particleGo.transform.position = projectile.transform.position;
            particleGo.SetActive(true);
            XIVEventSystem.SendEvent(new InvokeAfterEvent(5f).OnCompleted(() => particlePool.Release(particleGo)));
        }

        GameObject OnCreateParticle()
        {
            return Object.Instantiate(stateMachine.attackStateDataSO.projectilePrefab.projectileParticlePrefab);
        }

        static void OnReleaseParticle(GameObject go)
        {
            go.SetActive(false);
        }
    }
}
