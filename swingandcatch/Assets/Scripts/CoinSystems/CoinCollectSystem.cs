﻿using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems;
using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.Pool;
using XIV.Core;
using XIV.Core.Collections;
using XIV.Core.Extensions;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.EventSystem;
using XIV.EventSystem.Events;
using Random = UnityEngine.Random;

namespace TheGame.CoinSystems
{
    struct CollectCoinData
    {
        public Transform transform;
        public Vector3[] curve;
        public Timer timer;
    }
    
    public class CoinCollectSystem : MonoBehaviour
    {
        [SerializeField] CoinChannelSO coinTriggeredChannelSO;
        [SerializeField] GameObject coinCollectedParticlePrefab;
        [SerializeField] EasingFunction.Ease coinCollectEase;
        EasingFunction.Function easing;

        Camera cam;
        int collectedCoinCount;
        DynamicArray<CollectCoinData> coinDatas = new DynamicArray<CollectCoinData>();
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

        void Update()
        {
            int count = coinDatas.Count;
            if (count == 0) return;
            
            var deltaTime = Time.deltaTime;
            for (int i = count - 1; i >= 0; i--)
            {
                ref var coinData = ref coinDatas[i];
                var coinTransform = coinData.transform;
                var coinCurve = coinData.curve;
                coinData.timer.Update(deltaTime);
                var t = easing(0f, 1f, coinData.timer.NormalizedTime);
                var screenSpacePos = BezierMath.GetPoint(coinCurve[0], coinCurve[1], coinCurve[2], coinCurve[3], t);
                var worldPos = cam.ScreenToWorldPoint(screenSpacePos);
                
                coinTransform.position = worldPos;

                if (t >= 1f)
                {
                    OnCoinCollected(i);
                }
                
                XIVDebug.DrawBezier(coinCurve, t, 0.25f);
            }
        }

        void OnEnable() => coinTriggeredChannelSO.Register(OnCoinTriggered);
        void OnDisable() => coinTriggeredChannelSO.Unregister(OnCoinTriggered);

        void OnCoinTriggered(Coin coin)
        {
            coin.enabled = false;
            var coinTransform = coin.transform;
            var coinTransformPosition = coinTransform.position;
            var coinScreenPos = cam.WorldToScreenPoint(coinTransformPosition);
            var coinUIItemRectPosition = UISystem.GetUI<HudUI>().coinPageUI.coinUIItemRectPosition;
            coinDatas.Add() = new CollectCoinData
            {
                transform = coinTransform,
                curve = BezierMath.CreateCurve(coinScreenPos, coinUIItemRectPosition.SetZ(coinScreenPos.z), Random.value * 3f),
                timer = new Timer(Random.Range(0.5f, 1f)),
            };
            
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
            }).OnCanceled(() =>
            {
                coinCollectedParticlePool.Release(particleGo);
            }));
        }

        void OnCoinCollected(int index)
        {
            collectedCoinCount++;
            UISystem.GetUI<HudUI>().coinPageUI.ChangeDisplayAmount(collectedCoinCount);
            Destroy(coinDatas[index].transform.gameObject);
            coinDatas.RemoveAt(index);
        }
    }
}