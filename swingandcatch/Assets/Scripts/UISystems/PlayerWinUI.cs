using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TheGame.UISystems
{
    public class PlayerWinUI : GameUI
    {
        [SerializeField] Button btn_PlayAgain;

        void OnEnable()
        {
            btn_PlayAgain.onClick.AddListener(ReloadScene);
        }

        void OnDisable()
        {
            btn_PlayAgain.onClick.RemoveListener(ReloadScene);
        }

        void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}