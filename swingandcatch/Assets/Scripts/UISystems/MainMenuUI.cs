using System;
using TheGame.SaveSystems;
using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TheGame.UISystems
{
    public class MainMenuUI : ParentGameUI
    {
        [Header("UI Elements")]
        [SerializeField] Button btn_Start;
        [SerializeField] Button btn_Continue;
        [SerializeField] Button btn_Options;
        [SerializeField] Button btn_Exit;

        void Start()
        {
            // SaveManager.Instance.savedSceneIndex == 0 means player never played an actual level, 0 is MainMenu
            var activateContinueButton = SaveManager.Instance.hasSaveData && SaveManager.Instance.savedSceneIndex > 0;
            btn_Continue.gameObject.SetActive(activateContinueButton);
        }

        void OnEnable()
        {
            btn_Start.onClick.AddListener(StartNewGame);
            btn_Continue.onClick.AddListener(ContinueGame);
            btn_Options.onClick.AddListener(ShowOptionsPage);
            btn_Exit.onClick.AddListener(ExitGame);
        }

        void OnDisable()
        {
            btn_Start.onClick.RemoveListener(StartNewGame);
            btn_Continue.onClick.RemoveListener(ContinueGame);
            btn_Options.onClick.RemoveListener(ShowOptionsPage);
            btn_Exit.onClick.RemoveListener(ExitGame);
        }

        void StartNewGame()
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }

        void ContinueGame()
        {
            SceneManager.LoadScene(SaveManager.Instance.savedSceneIndex, LoadSceneMode.Single);
        }

        void ShowOptionsPage()
        {
            
        }

        void ExitGame()
        {
            Application.Quit();
        }
    }
}