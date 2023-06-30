using UnityEngine;
using UnityEngine.Pool;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.EventSystem;
using XIV.EventSystem.Events;

namespace TheGame.HazzardSystems
{
    public class BallLauncher : MonoBehaviour
    {
        [SerializeField] Transform ballLaunchPos;
        [SerializeField] float launchDuration;
        [SerializeField] float launchStartDelay = 0.5f;
        [SerializeField] float ballSpeed;
        [SerializeField] float damageAmount;
        [SerializeField] Projectile projectilePrefab;

        ObjectPool<Projectile> projectilePool;
        ObjectPool<GameObject> particlePool;

        Timer launchTimer;

        void Awake()
        {
            launchTimer = new Timer(launchDuration);

            projectilePool = new ObjectPool<Projectile>(CreateHazzardBall, null, ReleaseHazzardBall);
            particlePool = new ObjectPool<GameObject>(() => Instantiate(projectilePrefab.projectileParticlePrefab), null, (go) => go.SetActive(false));
        }

        void Start()
        {
            if (launchStartDelay > 0f) return;
            LaunchBall();
        }

        void Update()
        {
            if (launchStartDelay > 0f)
            {
                launchStartDelay -= Time.deltaTime;
                return;
            }

            launchTimer.Update(Time.deltaTime);
            var smoothNormalizedTime = XIVMathf.RemapClamped(EasingFunction.SmoothStop5(launchTimer.NormalizedTime), 0f, 1f, 0.5f, 1f);
            transform.localScale = Vector3.one * smoothNormalizedTime;

            if (launchTimer.IsDone)
            {
                LaunchBall();

                launchTimer.Restart(launchDuration);
            }
        }

        void LaunchBall()
        {
            Projectile projectile = projectilePool.Get();
            var layerMask = (1 << PhysicsConstants.PlayerLayer) | (1 << PhysicsConstants.GroundLayer) | (1 << PhysicsConstants.LavaLayer);
            projectile.Initialize(projectilePool, 15f, ballSpeed, damageAmount, transform.forward, layerMask);
            projectile.transform.position = ballLaunchPos.position;
            projectile.gameObject.SetActive(true);
        }
        
        Projectile CreateHazzardBall() => Instantiate(projectilePrefab);
        void ReleaseHazzardBall(Projectile projectile)
        {
            var particleGo = particlePool.Get();
            particleGo.transform.position = projectile.transform.position;
            XIVEventSystem.SendEvent(new InvokeAfterEvent(5f).OnCompleted(() => particlePool.Release(particleGo)));
            projectile.gameObject.SetActive(false);
            particleGo.SetActive(true);
        }
    }
}
