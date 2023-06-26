using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems;
using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.Pool;
using XIV.Core.Extensions;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.EventSystem;
using XIV.EventSystem.Events;
using Random = UnityEngine.Random;

namespace TheGame.CoinSystems
{
    public class CoinCollectSystem : MonoBehaviour
    {
        [SerializeField] IntChannelSO onCoinCollectedAmountChangedChannel;
        [SerializeField] CoinChannelSO coinTriggeredChannelSO;
        [SerializeField] GameObject coinCollectedParticlePrefab;
        [SerializeField] EasingFunction.Ease coinCollectEase;
        EasingFunction.Function easing;

        Camera cam;
        int collectedCoinCount;
        ObjectPool<GameObject> coinCollectedParticlePool;

        void Awake()
        {
            cam = Camera.main;
            easing = EasingFunction.GetEasingFunction(coinCollectEase);

            GameObject CreateParticle()
            {
                var go = Instantiate(coinCollectedParticlePrefab);
                go.SetActive(true);
                return go;
            }

            void OnReleasedParticle(GameObject particle) => particle.SetActive(false);

            coinCollectedParticlePool = new ObjectPool<GameObject>(CreateParticle, null, OnReleasedParticle, null, false);
        }

        void OnEnable() => coinTriggeredChannelSO.Register(OnCoinTriggered);
        void OnDisable() => coinTriggeredChannelSO.Unregister(OnCoinTriggered);

        void OnCoinTriggered(Coin coin)
        {
            coin.enabled = false;
            var coinTransform = coin.transform;
            var coinTransformPosition = coinTransform.position;
            var coinScreenPos = cam.WorldToScreenPoint(coinTransformPosition);
            var hud = UISystem.GetUI<HudUI>();
            var coinUIItemRectPosition = hud != null ? hud.coinUIItemRectPosition.SetZ(coinScreenPos.z) : (Vector3.one * 2f).SetZ(coinScreenPos.z);
            
            var curve = BezierMath.CreateCurve(coinScreenPos, coinUIItemRectPosition, Random.value * 3f);
            coinTransform.XIVTween()
                .WorldToUIMove(curve, Camera.main, Random.Range(5f, 8f), easing) // TODO : Decrease coin collect duration
                .OnComplete(OnCoinTweenCompleted)
                .Start();
            
            HandleParticle(coinTransformPosition);
        }

        void OnCoinTweenCompleted(GameObject go)
        {
            collectedCoinCount++;
            onCoinCollectedAmountChangedChannel.RaiseEvent(collectedCoinCount);
            if (go != null) Destroy(go);
        }
        
        void HandleParticle(Vector3 coinTransformPosition)
        {
            var particleGo = coinCollectedParticlePool.Get();
            particleGo.transform.position = coinTransformPosition;
            var particleSystems = particleGo.GetComponentsInChildren<ParticleSystem>(true);
            XIVEventSystem.SendEvent(new InvokeUntilEvent().AddCancelCondition(() =>
                {
                    for (int i = 0; i < particleSystems.Length; i++)
                    {
                        if (particleSystems[i].isStopped == false)
                        {
                            return false;
                        }
                    }

                    return true;
                })
                .OnCanceled(() => coinCollectedParticlePool.Release(particleGo)));
        }
        
    }
}