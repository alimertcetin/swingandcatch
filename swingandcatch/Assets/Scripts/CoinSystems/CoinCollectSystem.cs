using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems;
using TheGame.UISystems.Core;
using UnityEngine;
using XIV.Core.Extensions;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using Random = UnityEngine.Random;

namespace TheGame.CoinSystems
{
    public class CoinCollectSystem : MonoBehaviour
    {
        [SerializeField] IntChannelSO onCoinCollectedAmountChangedChannel;
        [SerializeField] CoinChannelSO coinTriggeredChannelSO;
        [SerializeField] EasingFunction.Ease coinCollectEase;
        EasingFunction.Function easing;

        Camera cam;
        int collectedCoinCount;

        void Awake()
        {
            cam = Camera.main;
            easing = EasingFunction.GetEasingFunction(coinCollectEase);
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
                .WorldToUIMove(curve, Camera.main, Random.Range(0.5f, 1f), easing)
                .OnComplete(OnCoinTweenCompleted)
                .Start();
        }

        void OnCoinTweenCompleted(GameObject go)
        {
            collectedCoinCount++;
            onCoinCollectedAmountChangedChannel.RaiseEvent(collectedCoinCount);
            if (go != null) Destroy(go);
        }
        
    }
}