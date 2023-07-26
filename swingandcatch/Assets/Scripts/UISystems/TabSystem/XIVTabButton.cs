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
    public class XIVTabButton : Selectable, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISubmitHandler
    {
        [SerializeField] Color pointerEnterColor;
        [SerializeField] Sprite pointerEnterSprite;
        
        [SerializeField] Color toggleColor;
        [SerializeField] Sprite toggleSprite;
        [Tooltip("Sets the current TabButton as Active GameObject for the EventSystem")]
        [SerializeField] bool setActiveObjectOnEnter;
        
        public event Action<XIVTabButton> onPointerEnter = delegate {  };
        public event Action<XIVTabButton> onPointerExit = delegate {  };
        public event Action<XIVTabButton> onPointerDown = delegate {  };
        public event Action<XIVTabButton> onPointerUp = delegate {  };
        
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
            HandleOnSelect();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            HandleOnDeselect();
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

        public override void OnSelect(BaseEventData eventData)
        {
            eventData.Use();
            HandleOnSelect();
            base.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            eventData.Use();
            HandleOnDeselect();
            base.OnDeselect(eventData);
        }

        void HandleOnSelect()
        {
            if (setActiveObjectOnEnter && EventSystem.current.currentSelectedGameObject != gameObject)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }

            onPointerEnter.Invoke(this);
            if (isOn)
            {
                SetImage(Color.Lerp(toggleColor, pointerEnterColor, 0.5f), pointerEnterSprite);
                return;
            }

            SetImage(pointerEnterColor, pointerEnterSprite);
        }

        void HandleOnDeselect()
        {
            onPointerExit.Invoke(this);
            if (isOn)
            {
                SetImage(toggleColor, toggleSprite);
                return;
            }

            SetImage(initialColor, initialSprite);
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