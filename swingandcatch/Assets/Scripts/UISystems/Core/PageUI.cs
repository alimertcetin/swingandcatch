using UnityEngine;
using XIV.Core.Utils;
using XIV.Core.TweenSystem;

namespace TheGame.UISystems.Core
{
    public abstract class PageUI : MonoBehaviour
    {
        [SerializeField] protected GameObject uiGameObject;
        public bool isActive { get; protected set; }

        protected virtual void Awake()
        {
            isActive = uiGameObject.activeSelf;
        }

        public virtual void Show()
        {
            uiGameObject.transform.localScale = Vector3.zero;
            uiGameObject.SetActive(true);
            uiGameObject.transform.CancelTween();
            uiGameObject.transform.XIVTween()
                .Scale(Vector3.zero, Vector3.one, 0.5f, EasingFunction.EaseInOutExpo)
                .UseUnscaledDeltaTime()
                .OnComplete(() =>
                {
                    isActive = true;
                    OnUIActivated();
                })
                .Start();
        }

        public virtual void Hide()
        {
            uiGameObject.transform.CancelTween();
            uiGameObject.SetActive(false);
            isActive = false;
        }

        protected virtual void OnUIActivated() { }
        protected virtual void OnUIDeactivated() { }
        
    }

}