using TheGame.UISystems.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XIV.Core.Utils;
using XIV.Core.TweenSystem;

namespace TheGame.UISystems
{
    public class HudHealthPageUI : PageUI
    {
        [SerializeField] Image image;
        Color initialColor;

        protected override void Awake()
        {
            base.Awake();
            initialColor = image.color;
        }

        public void ChangeDisplayAmount(float newAmount)
        {
            if (isActive == false)
            {
                image.fillAmount = newAmount;
                return;
            }

            image.CancelTween();
            var current = image.fillAmount;
            var duration = 0.5f;
            var currentColor = image.color;
            var targetColor = Color.Lerp(Color.red, initialColor, newAmount);
            image.XIVTween()
                .ImageFill(current, newAmount, duration, EasingFunction.Linear)
                .And()
                .ImageColor(currentColor, targetColor, duration, EasingFunction.Linear)
                .Start();
        }
    }
}