using UnityEngine;
using XIV.Core;
using XIV.Core.Utils;
using XIV.Core.TweenSystem;

namespace TheGame.UISystems.Core
{
    public abstract class GameUI : MonoBehaviour
    {
        [SerializeField] protected GameObject uiGameObject;
        protected RectTransform uiGameObjectRectTransform;
        
        public bool isActive { get; protected set; }
        
        protected virtual void Awake()
        {
            UISystem.AddUI(this);
            uiGameObjectRectTransform = uiGameObject.transform.GetComponent<RectTransform>();
        }

#if UNITY_EDITOR
        [Button(true)]
#endif
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


#if UNITY_EDITOR
        [Button(true)]
#endif
        public virtual void Hide()
        {
            uiGameObject.transform.CancelTween();
            uiGameObject.transform.XIVTween()
                .Scale(Vector3.one, Vector3.zero, 0.5f, EasingFunction.EaseInOutExpo)
                .UseUnscaledDeltaTime()
                .OnComplete(() =>
                {
                    isActive = false;
                    uiGameObject.SetActive(false);
                    OnUIDeactivated();
                })
                .Start();
        }

        /// <summary>
        /// Gets called when Show tween ends
        /// </summary>
        protected virtual void OnUIActivated() { }
        
        /// <summary>
        /// Gets called when Hide tween ends
        /// </summary>
        protected virtual void OnUIDeactivated() { }

        protected virtual void OnDestroy() => UISystem.RemoveUI(this);
    }
}