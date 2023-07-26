using TheGame.UISystems.Core;
using UnityEngine;
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
        public XIVTabButton tabButton;
    }

    public class TabUI : GameUI
    {
        [SerializeField] protected PageData[] pages;
        [SerializeField] protected TabMovementDirection tabMovementDirection = TabMovementDirection.Horizontal;
        int pagesLength;
        int currentPageIndex;

        protected override void Awake()
        {
            pagesLength = pages.Length;
            base.Awake();
        }

        protected virtual void OnEnable()
        {
            for (int i = 0; i < pagesLength; i++)
            {
                pages[i].tabButton.onPointerDown += OnTabButtonPressed;
            }
        }

        protected virtual void OnDisable()
        {
            for (int i = 0; i < pagesLength; i++)
            {
                pages[i].tabButton.onPointerDown -= OnTabButtonPressed;
            }
        }
        
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

        protected virtual void OpenPage(int index)
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

        protected virtual void ClosePage(int targetPageIndex)
        {
            var targetPage = pages[targetPageIndex];
            var currentPage = pages[currentPageIndex];

            var targetPageButtonPos = targetPage.tabButton.rectTransform.anchoredPosition;
            var currentPageButtonPos = currentPage.tabButton.rectTransform.anchoredPosition;
            TabHelper.GetDirections(tabMovementDirection, targetPageButtonPos, currentPageButtonPos, out var targetEnterDir, out var currentExitDir);

            targetPage.tabPageUI.enterDirection = targetEnterDir;
            currentPage.tabPageUI.exitDirection = currentExitDir;
            
            currentPage.tabPageUI.Hide();
            currentPage.tabButton.ReleaseToggle();
        }

        protected virtual void OnTabButtonPressed(XIVTabButton tabButton)
        {
            var pageIndex = IndexOfPageData(tabButton);
            if (pageIndex == currentPageIndex)
            {
                pages[pageIndex].tabPageUI.OnFocus();
                return;
            }

            OpenPage(pageIndex);
        }

        protected int IndexOfPageData(XIVTabButton tabButton)
        {
            for (int i = 0; i < pagesLength; i++)
            {
                if (pages[i].tabButton == tabButton) return i;
            }

            return default;
        }
    }
}