using TheGame.UISystems.Core;
using TMPro;
using UnityEngine;
using XIV.Core.Utils;
using XIV.TweenSystem;

namespace TheGame.UISystems
{
    public class HudCoinPageUI : PageUI
    {
        [SerializeField] RectTransform coinUIItemRect;
        [SerializeField] TMP_Text coinText;
        public Vector3 coinUIItemRectPosition => coinUIItemRect.position;

        public void ChangeDisplayAmount(int newAmount)
        {
            coinText.text = newAmount.ToString();
            coinUIItemRect.CancelTween(false);
            var localScale = coinUIItemRect.localScale;
            coinUIItemRect.XIVTween()
                .Scale(localScale, localScale + Vector3.one * 0.25f, 0.1f, EasingFunction.EaseInOutCubic)
                .OnComplete(() =>
                {
                    var currentScale = coinUIItemRect.localScale;
                    var diff = Vector3.Distance(currentScale, Vector3.one);
                    coinUIItemRect.XIVTween()
                        .Scale(currentScale, Vector3.one, diff * 0.5f, EasingFunction.EaseOutElastic)
                        .Start();
                })
                .Start();
        }
    }
}