using System;
using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TheGame.UISystems
{
    public class PlayerDiedUI : GameUI
    {
        [SerializeField] Button btn_Restart;

        void OnEnable()
        {
            btn_Restart.onClick.AddListener(ReloadScene);
        }

        void OnDisable()
        {
            btn_Restart.onClick.RemoveListener(ReloadScene);
        }

        void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}