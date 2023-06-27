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
        [SerializeField] float ballSpeed;
        [SerializeField] HazzardBall hazzardBallPrefab;
        
        Timer launchTimer;

        ObjectPool<HazzardBall> hazzardBallPool;

        void Awake()
        {
            launchTimer = new Timer(launchDuration);
            hazzardBallPool = new ObjectPool<HazzardBall>(OnCreateHazzardBall, null, OnReleaseHazzardBall);
        }

        HazzardBall OnCreateHazzardBall() => Instantiate(hazzardBallPrefab);
        void OnReleaseHazzardBall(HazzardBall hazzardBall) => hazzardBall.gameObject.SetActive(false);

        void Update()
        {
            launchTimer.Update(Time.deltaTime);
            var smoothNormalizedTime = XIVMathf.RemapClamped(EasingFunction.SmoothStop5(launchTimer.NormalizedTime), 0f, 1f, 0.5f, 1f);
            transform.localScale = Vector3.one * smoothNormalizedTime;

            if (launchTimer.IsDone)
            {
                HazzardBall hazzardBall = hazzardBallPool.Get();
                hazzardBall.speed = ballSpeed;
                hazzardBall.direction = transform.forward;
                hazzardBall.onOutsideOfTheView = () => ReleaseHazzardBall(hazzardBall);
                hazzardBall.transform.position = ballLaunchPos.position;
                hazzardBall.gameObject.SetActive(true);
                
                var hazzardMono = hazzardBall.GetComponent<HazzardMono>();
                hazzardMono.RegisterHit(OnHazzardBallHit);

                void OnHazzardBallHit(Transform t)
                {
                    hazzardMono.UnregisterHit(OnHazzardBallHit);
                    ReleaseHazzardBall(hazzardBall);
                }
                
                launchTimer.Restart(launchDuration);
            }
        }

        void ReleaseHazzardBall(HazzardBall hazzardBall)
        {
            hazzardBallPool.Release(hazzardBall);
        }
    }
}
