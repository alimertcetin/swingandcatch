using System.Collections.Generic;
using TheGame.SceneManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.ScriptableObjects.SceneManagement;
using TheGame.Scripts.InputSystems;
using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TheGame.UISystems
{
    public class PauseUI : GameUI
    {
        [SerializeField] BoolChannelSO showPauseUIChannel;
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
        [SerializeField] SceneListSO sceneListSO;
        
        [SerializeField] Button btn_Resume;
        [SerializeField] Button btn_MainMenu;
        [SerializeField] BoolChannelSO gamePausedChannel;
        
        float previousTimeScale;
        
        void OnEnable()
        {
            btn_Resume.onClick.AddListener(Resume);
            btn_MainMenu.onClick.AddListener(GoToMainMenu);
        }

        void OnDisable()
        {
            btn_Resume.onClick.RemoveListener(Resume);
            btn_MainMenu.onClick.RemoveListener(GoToMainMenu);
        }

        public override void Show()
        {
            base.Show();
            InputManager.DisableAll();
            InputManager.Inputs.PauseUI.Enable();
            InputManager.Inputs.PauseUI.Resume.performed += OnPauseUIResumePerformed;
        }

        public override void Hide()
        {
            InputManager.Inputs.InGame.Enable();
            InputManager.Inputs.PauseUI.Disable();
            InputManager.Inputs.PauseUI.Resume.performed -= OnPauseUIResumePerformed;
            // https://docs.unity3d.com/ScriptReference/Time-timeScale.html WTF?!
            Time.timeScale = previousTimeScale;
            gamePausedChannel.RaiseEvent(false);
            base.Hide();
        }

        protected override void OnUIActivated()
        {
            gamePausedChannel.RaiseEvent(true);
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        void OnPauseUIResumePerformed(InputAction.CallbackContext obj)
        {
            Resume();
        }

        void Resume()
        {
            showPauseUIChannel.RaiseEvent(false);
        }

        void GoToMainMenu()
        {
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.MenuLoad(sceneListSO.mainMenuSceneIndex));
        }
    }
}