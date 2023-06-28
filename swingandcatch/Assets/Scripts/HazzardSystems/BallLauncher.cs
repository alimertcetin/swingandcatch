using UnityEngine;
using UnityEngine.Pool;
using XIV.Core.Utils;
using XIV.Core.XIVMath;

namespace TheGame.HazzardSystems
{
    public class BallLauncher : MonoBehaviour
    {
        [SerializeField] Transform ballLaunchPos;
        [SerializeField] float launchDuration;
        [SerializeField] float launchStartDelay = 0.5f;
        [SerializeField] float ballSpeed;
        [SerializeField] HazzardBall hazzardBallPrefab;

        ObjectPool<HazzardBall> hazzardBallPool;

        Timer launchTimer;

        void Awake()
        {
            launchTimer = new Timer(launchDuration);

            hazzardBallPool = new ObjectPool<HazzardBall>(CreateHazzardBall, GetHazzardBall, ReleaseHazzardBall);
        }

        void Start()
        {
            if (launchStartDelay > 0f) return;
            LaunchBall();
        }

        void Update()
        {
            if (launchStartDelay > 0f) launchStartDelay -= Time.deltaTime;
            if (launchStartDelay > 0f) return;
            
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
            HazzardBall hazzardBall = hazzardBallPool.Get();
            hazzardBall.speed = ballSpeed;
            hazzardBall.direction = transform.forward;
            hazzardBall.transform.position = ballLaunchPos.position;
            hazzardBall.obstacleLayerMask = 1 << PhysicsConstants.GroundLayer;

            var hazzardMono = hazzardBall.GetComponent<HazzardMono>();
            RegisterEvents(hazzardBall);

        }
        
        void OnHitObstacle(HazzardBall hazzardBall)
        {
            UnregisterEvents(hazzardBall);
                
            var particleGo = Instantiate(hazzardBall.particlePrefab);
            particleGo.transform.position = hazzardBall.transform.position;
            Destroy(particleGo, 5f);
            hazzardBallPool.Release(hazzardBall);
        }

        void OnOutsideOfTheView(HazzardBall hazzardBall)
        {
            UnregisterEvents(hazzardBall);
            hazzardBallPool.Release(hazzardBall);
        }

        void OnHazzardMonoHit(HazzardMono hazzardMono, Transform other)
        {
            var hazzardBall = hazzardMono.GetComponent<HazzardBall>();
            
            UnregisterEvents(hazzardBall);

            var particleGo = Instantiate(hazzardBall.particlePrefab);
            particleGo.transform.position = hazzardBall.transform.position;
            Destroy(particleGo, 5f);
            hazzardBallPool.Release(hazzardBall);
        }

        void RegisterEvents(HazzardBall hazzardBall)
        {
            hazzardBall.onHitObstacle += OnHitObstacle;
            hazzardBall.onOutsideOfTheView += OnOutsideOfTheView;
            hazzardBall.GetComponent<HazzardMono>().RegisterHit(OnHazzardMonoHit);
        }

        void UnregisterEvents(HazzardBall hazzardBall)
        {
            hazzardBall.onHitObstacle -= OnHitObstacle;
            hazzardBall.onOutsideOfTheView -= OnOutsideOfTheView;
            hazzardBall.GetComponent<HazzardMono>().UnregisterHit(OnHazzardMonoHit);
        }
        
        HazzardBall CreateHazzardBall() => Instantiate(hazzardBallPrefab);
        void GetHazzardBall(HazzardBall hazzardBall) => hazzardBall.gameObject.SetActive(true);
        void ReleaseHazzardBall(HazzardBall hazzardBall) => hazzardBall.gameObject.SetActive(false);
    }
}
