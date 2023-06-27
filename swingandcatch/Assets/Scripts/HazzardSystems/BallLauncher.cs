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
            hazzardBall.onOutsideOfTheView = () => { };
            hazzardBall.transform.position = ballLaunchPos.position;

            var hazzardMono = hazzardBall.GetComponent<HazzardMono>();
            hazzardMono.RegisterHit(OnHazzardBallHit);

            void OnHazzardBallHit(Transform t)
            {
                hazzardMono.UnregisterHit(OnHazzardBallHit);
                var particleGo = Instantiate(hazzardBall.particlePrefab);
                Destroy(particleGo, 5f);
            }
        }
    }
}
