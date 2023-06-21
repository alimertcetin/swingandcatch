using System;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.UI;
using XIV.Core.Utils;
using XIV.EventSystem;
using XIV.EventSystem.Events;

namespace TheGame.UISystems
{
    public class HudHealthPageUI : PageUI
    {
        [SerializeField] Image image;
        [SerializeField] FloatChannelSO updatePlayerHealthChannel;
        float currentAmount;
        const float IMAGE_FILL_DURATION = 0.5f;
        InvokeForSecondsEvent fillEvent;

        protected override void Awake()
        {
            base.Awake();
            ChangeDisplayAmount(1f);
        }

        void OnEnable() => updatePlayerHealthChannel.Register(ChangeDisplayAmount);
        void OnDisable() => updatePlayerHealthChannel.Unregister(ChangeDisplayAmount);

        public void ChangeDisplayAmount(float newAmount)
        {
            if (fillEvent != null) XIVEventSystem.CancelEvent(fillEvent);

            if (isActive == false)
            {
                currentAmount = newAmount;
                image.material.SetFloat(ShaderConstants.Unlit_HealthbarShader.Health_RangeID, currentAmount);
                return;
            }
            
            fillEvent = new InvokeForSecondsEvent(IMAGE_FILL_DURATION)
                .AddAction((timer) =>
                {
                    var easedTime = EasingFunction.Linear(timer.NormalizedTime);
                    var healthAmount = Mathf.Lerp(currentAmount, newAmount, easedTime);
                    image.material.SetFloat(ShaderConstants.Unlit_HealthbarShader.Health_RangeID, healthAmount);
                })
                .OnCanceled(SetHealthToNewAmount)
                .OnCompleted(SetHealthToNewAmount);
            
            void SetHealthToNewAmount()
            {
                currentAmount = newAmount;
            }

            XIVEventSystem.SendEvent(fillEvent);
        }

        void OnDestroy()
        {
            image.material.SetFloat(ShaderConstants.Unlit_HealthbarShader.Health_RangeID, 1f);
        }
    }
}