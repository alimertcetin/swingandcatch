using TheGame.ScriptableObjects.Channels;
using TMPro;
using UnityEngine;
using XIV.Core;
using XIV.Core.Collections;
using XIV.Core.Extensions;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.TweenSystem;
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
        [SerializeField] CoinChannelSO coinCollectedChannelSO;
        [SerializeField] RectTransform coinUIItem;
        [SerializeField] TMP_Text coinText;
        [SerializeField] EasingFunction.Ease coinCollectEase;
        EasingFunction.Function easing;

        Camera cam;
        int collectedCoinCount;
        DynamicArray<CollectCoinData> coinDatas = new DynamicArray<CollectCoinData>();

        void Awake()
        {
            cam = Camera.main;
            easing = EasingFunction.GetEasingFunction(coinCollectEase);
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

        void OnEnable() => coinCollectedChannelSO.Register(OnCoinTriggered);
        void OnDisable() => coinCollectedChannelSO.Unregister(OnCoinTriggered);

        void OnCoinTriggered(Coin coin)
        {
            coin.enabled = false;
            var coinTransform = coin.transform;
            var coinTransformPosition = coinTransform.position;
            var coinScreenPos = cam.WorldToScreenPoint(coinTransformPosition);
            coinDatas.Add() = new CollectCoinData
            {
                transform = coinTransform,
                curve = BezierMath.CreateCurve(coinScreenPos, coinUIItem.position.SetZ(coinScreenPos.z), Random.value * 3f),
                timer = new Timer(Random.Range(0.5f, 1f)),
            };
        }

        void OnCoinCollected(int index)
        {
            collectedCoinCount++;
            coinText.text = collectedCoinCount.ToString();
            
            Destroy(coinDatas[index].transform.gameObject);
            coinDatas.RemoveAt(index);
            
            coinUIItem.CancelTween();
            coinUIItem.XIVTween()
                .Scale(Vector3.one, Vector3.one * 1.25f, 0.18f, EasingFunction.EaseInCubic, true)
                .Start();
        }
    }
}