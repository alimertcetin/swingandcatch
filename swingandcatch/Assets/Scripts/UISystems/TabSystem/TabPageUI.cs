using System;
using TheGame.UISystems.Core;
using UnityEngine;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;

namespace TheGame.UISystems.TabSystem
{
    public enum Direction
    {
        Right = 0,
        Left,
        Up,
        Down
    }
    
    public class TabPageUI : PageUI
    {
        public Direction enterDirection = Direction.Right;
        public Direction exitDirection = Direction.Left;
        [SerializeField] Vector2 fadeInOffset;
        [SerializeField] Vector2 fadeOutOffset;

        RectTransform rectTransform;
        
        protected override void Awake()
        {
            rectTransform = base.uiGameObject.GetComponent<RectTransform>();
            base.Awake();
        }

        public override void Show()
        {
            var vec = GetDirectionVector2(enterDirection);
            var outOfScreenPos = vec * rectTransform.rect.width + fadeInOffset;
            
            rectTransform.anchoredPosition = outOfScreenPos;
            
            rectTransform.CancelTween();
            rectTransform.XIVTween()
                .RectTransformMove(outOfScreenPos, Vector2.zero, 0.5f, EasingFunction.EaseOutExpo)
                .OnComplete(() =>
                {
                    isActive = true;
                    OnUIActivated();
                })
                .Start();
            
            base.uiGameObject.SetActive(true);
        }

        public override void Hide()
        {
            var vec = GetDirectionVector2(exitDirection);
            var outOfScreenPos = vec * rectTransform.rect.width + fadeOutOffset;
            rectTransform.CancelTween();
            rectTransform.XIVTween()
                .RectTransformMove(Vector2.zero, outOfScreenPos, 0.5f, EasingFunction.EaseOutExpo)
                .OnComplete(() =>
                {
                    uiGameObject.SetActive(false);
                    isActive = false;
                    OnUIDeactivated();
                })
                .Start();
        }

        public virtual void OnFocus() { }

        static Vector2 GetDirectionVector2(Direction direction)
        {
            return direction switch
            {
                Direction.Right => Vector2.right,
                Direction.Left => Vector2.left,
                Direction.Up => Vector2.up,
                Direction.Down => Vector2.down,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}