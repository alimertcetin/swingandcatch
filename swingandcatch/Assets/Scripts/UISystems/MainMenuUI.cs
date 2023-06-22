using TheGame.Data;
using TheGame.SaveSystems;
using TheGame.SceneManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UISystems
{
    public class MainMenuUI : ParentGameUI
    {
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
        
        [Header("UI Elements")]
        [SerializeField] Button btn_Start;
        [SerializeField] Button btn_Continue;
        [SerializeField] Button btn_Options;
        [SerializeField] Button btn_Exit;

        void Start()
        {
            // SaveManager.Instance.savedSceneIndex == 0,1,2 means player never played an actual level
            int savedSceneIndex = SaveManager.Instance.savedSceneIndex;
            var activateContinueButton = SaveManager.Instance.hasSaveData && (savedSceneIndex - 2) > 0;
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
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(GameData.SceneData.LEVEL_START_INDEX));
        }

        void ContinueGame()
        {
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(SaveManager.Instance.savedSceneIndex));
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