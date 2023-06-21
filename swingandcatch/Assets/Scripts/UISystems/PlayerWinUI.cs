using TheGame.Data;
using TheGame.SceneManagement;
using TheGame.ScriptableObjects;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TheGame.UISystems
{
    public class PlayerWinUI : GameUI
    {
        [SerializeField] LevelListSO levelListSO;
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
        [SerializeField] TMP_Text txt_Info;
        [SerializeField] Button btn_Continue;
        [SerializeField] Button btn_MainMenu;
        
        int nextLevel;

        void OnEnable()
        {
            btn_Continue.onClick.AddListener(LoadNextLevel);
            btn_MainMenu.onClick.AddListener(GoToMainMenu);
        }

        void OnDisable()
        {
            btn_Continue.onClick.RemoveListener(LoadNextLevel);
            btn_MainMenu.onClick.RemoveListener(GoToMainMenu);
        }

        public override void Show()
        {
            btn_Continue.gameObject.SetActive(false);
            btn_MainMenu.gameObject.SetActive(false);
            
            nextLevel = SceneManager.GetActiveScene().buildIndex + 1 - GameData.SceneData.LEVEL_START_INDEX;
            if (nextLevel > 0 && nextLevel <= levelListSO.buildIndices.Count)
            {
                nextLevel += GameData.SceneData.LEVEL_START_INDEX;
                txt_Info.text = "You Win! Continue to next level";
                btn_Continue.gameObject.SetActive(true);
            }
            else
            {
                txt_Info.text = "You have Completed all the levels!";
                btn_MainMenu.gameObject.SetActive(true);
            }
            
            base.Show();
        }

        void LoadNextLevel()
        {
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(nextLevel));
        }

        void GoToMainMenu()
        {
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.MenuLoad(GameData.SceneData.LEVEL_START_INDEX - 1));
        }
    }
}