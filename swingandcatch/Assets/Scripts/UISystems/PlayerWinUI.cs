using TheGame.Data;
using TheGame.SceneManagement;
using TheGame.ScriptableObjects;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using TMPro;
using UnityEngine;
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
            
            if (levelListSO.TryGetNextLevel(out nextLevel))
            {
                txt_Info.text = "You Win! Continue to next level";
                btn_Continue.gameObject.SetActive(true);
            }
            else
            {
                txt_Info.text = "You have Completed all the levels!";
            }
            
            base.Show();
        }

        void LoadNextLevel()
        {
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.LevelLoad(nextLevel));
        }

        void GoToMainMenu()
        {
            var options = SceneLoadOptions.MenuLoad(GameData.SceneData.MAIN_MENU);
            options.displayLoadingScreen = true;
            sceneLoadChannel.RaiseEvent(options);
        }
    }
}