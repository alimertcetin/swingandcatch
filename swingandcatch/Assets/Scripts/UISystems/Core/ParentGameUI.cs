using UnityEngine;
using XIV.Core.Utils;
using XIV.Core.TweenSystem;

namespace TheGame.UISystems.Core
{
    public abstract class ParentGameUI : GameUI
    {
        [SerializeField] protected GameObject mainPageGO;

        public override void Show()
        {
            base.Show();
            mainPageGO.transform.localScale = Vector3.one;
            mainPageGO.SetActive(true);
        }

        public override void Hide()
        {
            mainPageGO.transform.localScale = Vector3.one;
            uiGameObject.transform.CancelTween();
            uiGameObject.transform.XIVTween()
                .Scale(Vector3.one, Vector3.zero, 0.5f, EasingFunction.EaseInOutExpo)
                .UseUnscaledDeltaTime()
                .OnComplete(() =>
                {
                    mainPageGO.SetActive(false);
                    uiGameObject.SetActive(false);
                    OnUIDeactivated();
                })
                .Start();
            isActive = false;
        }

        public virtual void OpenPage(PageUI page)
        {
            mainPageGO.SetActive(false);
            page.Show();
        }

        public virtual void ComeBack(PageUI from)
        {
            mainPageGO.transform.localScale = Vector3.zero;
            mainPageGO.SetActive(true);
            mainPageGO.transform.CancelTween();
            mainPageGO.transform.XIVTween()
                .Scale(Vector3.zero, Vector3.one, 0.5f, EasingFunction.EaseInOutExpo)
                .Start();
        }
        
    }
}