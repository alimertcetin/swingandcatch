using TheGame.SceneManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Components;
using TheGame.UISystems.Core;
using UnityEngine;

namespace TheGame.UISystems
{
    public class MainMenuUI : ParentGameUI
    {
        [SerializeField] LevelDataChannelSO levelDataLoadedChannel;
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;

        [Header("Main Page UI Elements")]
        [SerializeField] CustomButton btn_Start;
        [SerializeField] CustomButton btn_Continue;
        [SerializeField] CustomButton btn_Options;
        [SerializeField] CustomButton btn_Exit;

        [Header("Audio")]
        [SerializeField] AudioPlayerSO mainMenuButtonPressAudioPlayer;
        [SerializeField] AudioPlayerSO mainMenuButtonSelectionAudioPlayer;

        LevelData levelData;

        void OnEnable()
        {
            btn_Start.onClick.AddListener(StartNewGame);
            btn_Continue.onClick.AddListener(ContinueGame);
            btn_Options.onClick.AddListener(ShowOptionsPage);
            btn_Exit.onClick.AddListener(ExitGame);

            btn_Start.RegisterOnSelect(PlayButtonSelected);
            btn_Continue.RegisterOnSelect(PlayButtonSelected);
            btn_Options.RegisterOnSelect(PlayButtonSelected);
            btn_Exit.RegisterOnSelect(PlayButtonSelected);
            
            levelDataLoadedChannel.Register(OnSceneListDataLoaded);
        }

        void OnDisable()
        {
            btn_Start.onClick.RemoveListener(StartNewGame);
            btn_Continue.onClick.RemoveListener(ContinueGame);
            btn_Options.onClick.RemoveListener(ShowOptionsPage);
            btn_Exit.onClick.RemoveListener(ExitGame);

            btn_Start.UnregisterOnSelect();
            btn_Continue.UnregisterOnSelect();
            btn_Options.UnregisterOnSelect();
            btn_Exit.UnregisterOnSelect();
            
            levelDataLoadedChannel.Unregister(OnSceneListDataLoaded);
        }

        void OnSceneListDataLoaded(LevelData levelData)
        {
            this.levelData = levelData;
            this.levelData.TryGetNextLevel(-1, out var firstLevelBuildIndex);
            btn_Continue.gameObject.SetActive(levelData.lastPlayedLevel != firstLevelBuildIndex);
        }

        void StartNewGame()
        {
            PlayButtonPress();
            levelData.TryGetNextLevel(-1, out var nextLevel);
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(nextLevel));
        }

        void ContinueGame()
        {
            PlayButtonPress();
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(levelData.lastPlayedLevel));
        }

        void ShowOptionsPage()
        {
            PlayButtonPress();
            UISystem.Hide<MainMenuUI>();
            UISystem.Show<MainMenuOptionsUI>();
        }

        void ExitGame()
        {
            PlayButtonPress();
            Application.Quit();
        }

        void PlayButtonSelected()
        {
            mainMenuButtonSelectionAudioPlayer.Play();
        }

        void PlayButtonPress()
        {
            mainMenuButtonPressAudioPlayer.Play();
        }
    }
}