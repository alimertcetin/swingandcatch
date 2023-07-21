using System;
using TheGame.SceneManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using UnityEngine;
using XIV.EventSystem;
using XIV.EventSystem.Events;

namespace TheGame.UISystems.SceneLoading
{
    public class SceneLoadingUI : GameUI
    {
        [SerializeField] FloatChannelSO sceneLoadingProgressChannel;
        [SerializeField] VoidChannelSO newSceneLoadedChannel;
        [SerializeField] VoidChannelSO activateNewlyLoadedScene;
        [SerializeField] BoolChannelSO activateLoadingScreenCamera;

        [SerializeField] SceneLoadingFooterUI footerUI;
        [SerializeField] LevelLoadingUI levelLoadingUI;
        [SerializeField] MenuLoadingUI menuLoadingUI;

        SceneLoadOptions sceneLoadOptions;
        LoadingUIBase currentLoadingDisplay;
        
        void OnEnable()
        {
            sceneLoadingProgressChannel.Register(UpdateProgressBar);
            newSceneLoadedChannel.Register(OnSceneLoaded);
        }

        void OnDisable()
        {
            sceneLoadingProgressChannel.Unregister(UpdateProgressBar);
            newSceneLoadedChannel.Unregister(OnSceneLoaded);
        }

        public override void Show()
        {
            isActive = true;
            uiGameObject.SetActive(true);
            OnUIActivated();
            activateLoadingScreenCamera.RaiseEvent(true);
        }

        protected override void OnUIActivated()
        {
            switch (sceneLoadOptions.loadingScreenType)
            {
                case LoadingScreenType.None:
                    return;
                case LoadingScreenType.LevelLoading:
                    currentLoadingDisplay = levelLoadingUI;
                    break;
                case LoadingScreenType.MenuLoading:
                    currentLoadingDisplay = menuLoadingUI;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            currentLoadingDisplay.Show();
        }

        public override void Hide()
        {
            if (currentLoadingDisplay == null || currentLoadingDisplay.isActive == false)
            {
                uiGameObject.SetActive(false);
                isActive = false;
                OnUIDeactivated();
                return;
            }

            XIVEventSystem.SendEvent(new InvokeUntilEvent()
                .AddCancelCondition(() => currentLoadingDisplay.isActive == false)
                .OnCompleted(() =>
                {
                    uiGameObject.SetActive(false);
                    isActive = false;
                    currentLoadingDisplay = null;
                    OnUIDeactivated();
                }));

            currentLoadingDisplay.Hide();
        }

        protected override void OnUIDeactivated()
        {
            activateLoadingScreenCamera.RaiseEvent(false);
        }

        public void SetSceneLoadingOptions(SceneLoadOptions sceneLoadOptions)
        {
            this.sceneLoadOptions = sceneLoadOptions;
        }

        void UpdateProgressBar(float value)
        {
            if (isActive == false || currentLoadingDisplay == false) return;
            currentLoadingDisplay.UpdateProgressBar(value);
        }

        void OnSceneLoaded()
        {
            if (sceneLoadOptions.activateImmediately) return;

            if (sceneLoadOptions.loadingScreenType == LoadingScreenType.LevelLoading)
            {
                footerUI.SetFooterText("Press Any Key to Play");
                footerUI.Show();
                footerUI.OnSceneLoaded();
            }
            
            bool terminate = false;
            XIVEventSystem.SendEvent(new InvokeUntilEvent().AddAction(() =>
                {
                    if (Input.anyKey)
                    {
                        terminate = true;
                        activateNewlyLoadedScene.RaiseEvent();
                        if (footerUI.isActive) footerUI.Hide();
                    }
                })
                .AddCancelCondition(() => terminate));
        }
    }
}
