using TheGame.SaveSystems;
using TheGame.SceneManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.ScriptableObjects.SceneManagement;
using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UISystems
{
    public class MainMenuUI : ParentGameUI
    {
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
        [SerializeField] SceneListSO sceneListSO;
        
        [Header("UI Elements")]
        [SerializeField] Button btn_Start;
        [SerializeField] Button btn_Continue;
        [SerializeField] Button btn_Options;
        [SerializeField] Button btn_Exit;

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
            sceneListSO.TryGetNextLevel(-1, out var nextLevel);
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(nextLevel));
        }

        void ContinueGame()
        {
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(sceneListSO.lastPlayedLevel));
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