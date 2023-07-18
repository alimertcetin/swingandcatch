using TheGame.AudioManagement;
using TheGame.SaveSystems;
using TheGame.SceneManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.ScriptableObjects.SceneManagement;
using TheGame.UISystems.Components;
using TheGame.UISystems.Core;
using UnityEngine;

namespace TheGame.UISystems
{
    public class MainMenuUI : ParentGameUI
    {
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
        [SerializeField] SceneListSO sceneListSO;

        [Header("Main Page UI Elements")]
        [SerializeField] CustomButton btn_Start;
        [SerializeField] CustomButton btn_Continue;
        [SerializeField] CustomButton btn_Options;
        [SerializeField] CustomButton btn_Exit;

        [Header("Audio")]
        [SerializeField] AudioPlayOptionsChannelSO audioPlayOptionsChannel;
        [SerializeField] AudioClip buttonPressedSound;
        [SerializeField] AudioClip buttonSelectionSound;

        void Start()
        {
            btn_Continue.gameObject.SetActive(SaveSystem.IsSaveExistsAny());
        }

        void OnEnable()
        {
            btn_Start.onClick.AddListener(StartNewGame);
            btn_Continue.onClick.AddListener(ContinueGame);
            btn_Options.onClick.AddListener(ShowOptionsPage);
            btn_Exit.onClick.AddListener(ExitGame);

            btn_Start.RegisterOnSelect(OnButtonSelected);
            btn_Continue.RegisterOnSelect(OnButtonSelected);
            btn_Options.RegisterOnSelect(OnButtonSelected);
            btn_Exit.RegisterOnSelect(OnButtonSelected);
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
        }

        void StartNewGame()
        {
            audioPlayOptionsChannel.RaiseEvent(AudioPlayOptions.MusicPlayOptions(null)); // stop current music
            PlayButtonEffect(buttonPressedSound);
            sceneListSO.TryGetNextLevel(-1, out var nextLevel);
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(nextLevel));
        }

        void ContinueGame()
        {
            PlayButtonEffect(buttonPressedSound);
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(sceneListSO.lastPlayedLevel));
        }

        void ShowOptionsPage()
        {
            PlayButtonEffect(buttonPressedSound);
            UISystem.Hide<MainMenuUI>();
            UISystem.Show<MainMenuOptionsUI>();
        }

        void ExitGame()
        {
            PlayButtonEffect(buttonPressedSound);
            Application.Quit();
        }

        void OnButtonSelected()
        {
            PlayButtonEffect(buttonSelectionSound);
        }

        void PlayButtonEffect(AudioClip clip)
        {
            audioPlayOptionsChannel.RaiseEvent(AudioPlayOptions.EffectPlayOptions(clip));
        }
    }
}