using System;
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
        
        Timer launchTimer;

        void Awake()
        {
            launchTimer = new Timer(launchDuration);
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
            HazzardBall hazzardBall = Instantiate(hazzardBallPrefab);
            hazzardBall.speed = ballSpeed;
            hazzardBall.direction = transform.forward;
            hazzardBall.transform.position = ballLaunchPos.position;
            hazzardBall.obstacleLayerMask = 1 << PhysicsConstants.GroundLayer;

            var hazzardMono = hazzardBall.GetComponent<HazzardMono>();
            RegisterEvents();

            void OnHitObstacle()
            {
                UnregisterEvents();
                
                var particleGo = Instantiate(hazzardBall.particlePrefab);
                particleGo.transform.position = hazzardBall.transform.position;
                Destroy(particleGo, 5f);
                Destroy(hazzardMono.gameObject);
            }

            void OnOutsideOfTheView()
            {
                UnregisterEvents();
                
                Destroy(hazzardMono.gameObject);
            }

            void OnHazzardBallHit(Transform t)
            {
                UnregisterEvents();
                
                var particleGo = Instantiate(hazzardBall.particlePrefab);
                particleGo.transform.position = hazzardBall.transform.position;
                Destroy(particleGo, 5f);
                Destroy(hazzardMono.gameObject);
            }

            void RegisterEvents()
            {
                hazzardBall.onHitObstacle += OnHitObstacle;
                hazzardBall.onOutsideOfTheView += OnOutsideOfTheView;
                hazzardMono.RegisterHit(OnHazzardBallHit);
            }

            void UnregisterEvents()
            {
                hazzardBall.onHitObstacle -= OnHitObstacle;
                hazzardBall.onOutsideOfTheView -= OnOutsideOfTheView;
                hazzardMono.UnregisterHit(OnHazzardBallHit);
            }
        }
    }
}
