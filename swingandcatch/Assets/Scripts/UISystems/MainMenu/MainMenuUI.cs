using TheGame.SaveSystems;
using TheGame.SceneManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Components;
using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using XIV.Core.TweenSystem;
using XIV.Core.Utils;

namespace TheGame.UISystems.MainMenu
{
    public class MainMenuUI : GameUI
    {
        [SerializeField] LevelDataChannelSO levelDataLoadedChannel;
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;

        [Header("Main Page UI Elements")]
        [SerializeField] CustomButton btn_Start;
        [SerializeField] CustomButton btn_Continue;
        [SerializeField] CustomButton btn_Settings;
        [SerializeField] CustomButton btn_Exit;

        [Header("Audio")]
        [SerializeField] AudioPlayerSO mainMenuButtonPressAudioPlayer;
        [SerializeField] AudioPlayerSO mainMenuButtonSelectionAudioPlayer;

        LevelData levelData;
        GameObject lastClickedButton;

        void OnEnable()
        {
            btn_Start.onClick.AddListener(StartNewGame);
            btn_Continue.onClick.AddListener(ContinueGame);
            btn_Settings.onClick.AddListener(ShowSettingsPage);
            btn_Exit.onClick.AddListener(ExitGame);

            btn_Start.RegisterOnSelect(OnButtonPressed);
            btn_Continue.RegisterOnSelect(OnButtonPressed);
            btn_Settings.RegisterOnSelect(OnButtonPressed);
            btn_Exit.RegisterOnSelect(OnButtonPressed);
            
            levelDataLoadedChannel.Register(OnSceneListDataLoaded);
        }

        void OnDisable()
        {
            btn_Start.onClick.RemoveListener(StartNewGame);
            btn_Continue.onClick.RemoveListener(ContinueGame);
            btn_Settings.onClick.RemoveListener(ShowSettingsPage);
            btn_Exit.onClick.RemoveListener(ExitGame);

            btn_Start.UnregisterOnSelect();
            btn_Continue.UnregisterOnSelect();
            btn_Settings.UnregisterOnSelect();
            btn_Exit.UnregisterOnSelect();
            
            levelDataLoadedChannel.Unregister(OnSceneListDataLoaded);
        }
        
        public override void Show()
        {
            var upPos = Vector2.up * (uiGameObjectRectTransform.rect.height + uiGameObjectRectTransform.offsetMin.y + 10f);
            uiGameObjectRectTransform.CancelTween();
            uiGameObjectRectTransform.anchoredPosition = upPos;
            
            uiGameObject.transform.XIVTween()
                .RectTransformMove(upPos, Vector2.zero, 0.5f, EasingFunction.EaseOutExpo)
                .UseUnscaledDeltaTime()
                .OnComplete(() =>
                {
                    isActive = true;
                    OnUIActivated();
                })
                .Start();
            uiGameObject.SetActive(true);
        }
        
        public override void Hide()
        {
            EventSystem.current.SetSelectedGameObject(null);
            
            uiGameObjectRectTransform.CancelTween();

            var upPos = Vector2.up * (uiGameObjectRectTransform.rect.height + uiGameObjectRectTransform.offsetMin.y + 10f);
            
            uiGameObject.transform.XIVTween()
                .RectTransformMove(Vector2.zero, upPos, 0.5f, EasingFunction.EaseOutExpo)
                .UseUnscaledDeltaTime()
                .OnComplete(() =>
                {
                    uiGameObject.SetActive(false);
                    isActive = false;
                    OnUIDeactivated();
                })
                .Start();
        }

        protected override void OnUIActivated()
        {
            EventSystem.current.SetSelectedGameObject(lastClickedButton ? lastClickedButton : btn_Start.gameObject);
        }

        void OnSceneListDataLoaded(LevelData levelData)
        {
            this.levelData = levelData;
            this.levelData.TryGetNextLevel(-1, out var firstLevelBuildIndex);
            btn_Continue.gameObject.SetActive(levelData.lastPlayedLevel != firstLevelBuildIndex);
        }

        void StartNewGame()
        {
            SaveSystem.ClearSaveDataAll();
            PlayButtonPressSound();
            levelData.TryGetNextLevel(-1, out var nextLevel);
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(nextLevel));
            lastClickedButton = btn_Start.gameObject;
        }

        void ContinueGame()
        {
            PlayButtonPressSound();
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(levelData.lastPlayedLevel));
            lastClickedButton = btn_Continue.gameObject;
        }

        void ShowSettingsPage()
        {
            lastClickedButton = btn_Settings.gameObject;
            PlayButtonPressSound();
            UISystem.Hide<MainMenuUI>();
            UISystem.Show<MainMenuSettingsTabUI>();
        }

        void ExitGame()
        {
            PlayButtonPressSound();
            Application.Quit();
        }

        void OnButtonPressed()
        {
            mainMenuButtonSelectionAudioPlayer.Play();
        }

        void PlayButtonPressSound()
        {
            mainMenuButtonPressAudioPlayer.Play();
        }
    }
}