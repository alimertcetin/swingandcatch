using TheGame.UISystems.Core;
using TMPro;
using UnityEngine;
using XIV.Core.Utils;
using XIV.EventSystem;
using XIV.EventSystem.Events;

namespace TheGame.UISystems.SceneLoading
{
    public class SceneLoadingFooterUI : PageUI
    {
        [SerializeField] TMP_Text txt_Footer;

        public void SetFooterText(string text)
        {
            txt_Footer.text = text;
        }

        public void OnSceneLoaded()
        {
            var timer = new Timer(2f);
            XIVEventSystem.SendEvent(new InvokeUntilEvent().AddAction(() =>
                {
                    timer.Update(Time.unscaledDeltaTime);
                    txt_Footer.alpha = Mathf.Clamp01(timer.NormalizedTimePingPong + 0.5f);
                    if (timer.IsDone)
                    {
                        timer.Restart();
                    }
                })
                .AddCancelCondition(() => isActive == false));
        }
    }
}