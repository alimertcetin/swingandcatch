using System;
using TheGame.UISystems.Core;
using UnityEngine;
using XIV.Core;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;
using XIV.EventSystem;
using XIV.EventSystem.Events;

namespace TheGame.UISystems.TabSystem
{
    [System.Serializable]
    public struct PageData
    {
        public TabPageUI tabPageUI;
        public TabButton tabButton;
    }
    
    public class TabUI : GameUI
    {
        [SerializeField] PageData[] pages;
        int pagesLength;
        int currentPageIndex;

        protected override void Awake()
        {
            pagesLength = pages.Length;
            base.Awake();
        }

        void OnEnable()
        {
            for (int i = 0; i < pagesLength; i++)
            {
                pages[i].tabButton.onPointerDown += OnTabButtonPressed;
            }
        }

        void OnDisable()
        {
            for (int i = 0; i < pagesLength; i++)
            {
                pages[i].tabButton.onPointerDown -= OnTabButtonPressed;
            }
        }
        
        [Button(playModeOnly = true)]
        public override void Show()
        {
            var upPos = Vector2.up * (uiGameObjectRectTransform.rect.height + uiGameObjectRectTransform.offsetMin.y + 10f);
            uiGameObjectRectTransform.CancelTween();
            uiGameObjectRectTransform.anchoredPosition = upPos;
            
            XIVEventSystem.SendEvent(new InvokeAfterEvent(0.25f).OnCompleted(OnUIActivated));
            uiGameObject.transform.XIVTween()
                .RectTransformMove(upPos, Vector2.zero, 0.5f, EasingFunction.EaseOutExpo)
                .UseUnscaledDeltaTime()
                .OnComplete(() =>
                {
                    isActive = true;
                    // BUG : Recursive tween call causes issues. Null ref or StackOverflow exception
                    // XIVEventSystem.SendEvent(new InvokeAfterEvent(0.1f).OnCompleted(OnUIActivated));
                })
                .Start();
            uiGameObject.SetActive(true);
        }
        
        [Button(playModeOnly = true)]
        public override void Hide()
        {
            uiGameObjectRectTransform.CancelTween();

            var upPos = Vector2.up * (uiGameObjectRectTransform.rect.height + uiGameObjectRectTransform.offsetMin.y + 10f);
            
            // Recursive tween call causes issues. Null ref or StackOverflow exception
            XIVEventSystem.SendEvent(new InvokeAfterEvent(0.25f).OnCompleted(OnUIDeactivated));
            uiGameObject.transform.XIVTween()
                .RectTransformMove(Vector2.zero, upPos, 0.5f, EasingFunction.EaseOutExpo)
                .UseUnscaledDeltaTime()
                .OnComplete(() =>
                {
                    uiGameObject.SetActive(false);
                    isActive = false;
                })
                .Start();
        }

        protected override void OnUIActivated()
        {
            currentPageIndex = -1;
            OpenPage(0);
        }

        protected override void OnUIDeactivated()
        {
            var currentPage = pages[currentPageIndex];
            currentPage.tabPageUI.Hide();
            currentPage.tabButton.ReleaseToggle();
        }

        void OpenPage(int index)
        {
            var targetPage = pages[index];

            if (currentPageIndex == -1)
            {
                targetPage.tabPageUI.enterDirection = Direction.Down;
            }
            else
            {
                ClosePage(index);
            }
            targetPage.tabPageUI.Show();
            targetPage.tabButton.Toggle();
            
            currentPageIndex = index;
        }

        void ClosePage(int targetPageIndex)
        {
            var targetPage = pages[targetPageIndex];
            var currentPage = pages[currentPageIndex];
            
            var targetPageX = targetPage.tabButton.rectTransform.anchoredPosition.x;
            var currentPageX = currentPage.tabButton.rectTransform.anchoredPosition.x;
            var targetEnterDir = targetPageX > currentPageX ? Direction.Right : Direction.Left;
            var currentExitDir = targetPageX > currentPageX ? Direction.Left : Direction.Right;
            
            targetPage.tabPageUI.enterDirection = targetEnterDir;
            currentPage.tabPageUI.exitDirection = currentExitDir;
            
            currentPage.tabPageUI.Hide();
            currentPage.tabButton.ReleaseToggle();
        }

        void OnTabButtonPressed(TabButton tabButton)
        {
            var pageIndex = IndexOfPageData(tabButton);
            if (pageIndex == currentPageIndex) return;
            OpenPage(pageIndex);
        }

        int IndexOfPageData(TabButton tabButton)
        {
            for (int i = 0; i < pagesLength; i++)
            {
                if (pages[i].tabButton == tabButton) return i;
            }

            return default;
        }
    }
}