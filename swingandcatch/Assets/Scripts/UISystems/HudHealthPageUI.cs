using TheGame.UISystems.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XIV.Core.Utils;
using XIV.TweenSystem;

namespace TheGame.UISystems
{
    public class HudHealthPageUI : PageUI
    {
        [SerializeField] Image image;

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
            image.XIVTween()
                .ImageFill(current, newAmount, duration, EasingFunction.Linear)
                .Start();
        }
    }
}