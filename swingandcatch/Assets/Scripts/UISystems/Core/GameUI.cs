using UnityEngine;
using XIV.Core.Utils;
using XIV.Core.TweenSystem;

namespace TheGame.UISystems.Core
{
    public abstract class GameUI : MonoBehaviour
    {
        [SerializeField] protected GameObject uiGameObject;
        public bool isActive { get; protected set; }
        protected virtual void Awake() => UISystem.AddUI(this);
        public virtual void Show()
        {
            uiGameObject.transform.localScale = Vector3.zero;
            uiGameObject.SetActive(true);
            uiGameObject.transform.CancelTween();
            uiGameObject.transform.XIVTween()
                .Scale(Vector3.zero, Vector3.one, 0.5f, EasingFunction.EaseInOutExpo)
                .Start();
            isActive = true;
        }

        public virtual void Hide()
        {
            uiGameObject.transform.CancelTween();
            uiGameObject.transform.XIVTween()
                .Scale(Vector3.one, Vector3.zero, 0.5f, EasingFunction.EaseInOutExpo)
                .OnComplete(() =>
                {
                    uiGameObject.SetActive(false);
                })
                .Start();
            isActive = false;
        }

        protected virtual void OnDestroy() => UISystem.RemoveUI(this);
    }
}