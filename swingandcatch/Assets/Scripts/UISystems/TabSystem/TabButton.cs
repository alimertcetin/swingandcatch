using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XIV.Core;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;

namespace TheGame.UISystems.TabSystem
{
    [RequireComponent(typeof(Image))]
    public class TabButton : Selectable, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISubmitHandler
    {
        [SerializeField] Color pointerEnterColor;
        [SerializeField] Sprite pointerEnterSprite;
        
        [SerializeField] Color toggleColor;
        [SerializeField] Sprite toggleSprite;
        [Tooltip("Sets the current TabButton as Active GameObject for the EventSystem")]
        [SerializeField] bool setActiveObjectOnEnter;
        
        public event Action<TabButton> onPointerEnter = delegate {  };
        public event Action<TabButton> onPointerExit = delegate {  };
        public event Action<TabButton> onPointerDown = delegate {  };
        public event Action<TabButton> onPointerUp = delegate {  };
        
        [field : SerializeField, DisplayWithoutEdit]
        public bool isOn { get; private set; }

        public RectTransform rectTransform => image.rectTransform;

        Color initialColor;
        Sprite initialSprite;
        Image image;

        protected override void Awake()
        {
            base.Awake();
            image = GetComponent<Image>();
            initialColor = image.color;
            initialSprite = image.sprite;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (setActiveObjectOnEnter) EventSystem.current.SetSelectedGameObject(this.gameObject);
            onPointerEnter.Invoke(this);
            if (isOn) return;
            SetImage(pointerEnterColor, pointerEnterSprite);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            onPointerExit.Invoke(this);
            if (isOn) return;
            SetImage(initialColor, initialSprite);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            onPointerDown.Invoke(this);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            onPointerUp.Invoke(this);
        }

        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            onPointerDown.Invoke(this); // press
            onPointerUp.Invoke(this); // release
        }

        public void Toggle()
        {
            isOn = true;
            SetImage(toggleColor, toggleSprite);
        }

        public void ReleaseToggle()
        {
            isOn = false;
            SetImage(initialColor, initialSprite);
        }

        void SetImage(Color color = default, Sprite sprite = default)
        {
            color = color == default ? image.color : color;
            transform.CancelTween();
            transform.XIVTween()
                .ImageColor(image.color, color, 0.2f, EasingFunction.EaseOutExpo)
                .Start();
            image.sprite = sprite ? sprite : image.sprite;
        }
    }
}