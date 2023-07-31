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
    public class PauseUI : GameUI, DefaultGameInputs.IPauseUIActions
    {
        [SerializeField] BoolChannelSO showPauseUIChannel;
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
        [SerializeField] SceneListSO sceneListSO;
        
        [SerializeField] Button btn_Resume;
        [SerializeField] Button btn_MainMenu;
        [SerializeField] BoolChannelSO gamePausedChannel;
        
        float previousTimeScale;

        protected override void Awake()
        {
            InputManager.Inputs.PauseUI.SetCallbacks(this);
            base.Awake();
        }

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
            InputManager.DisableAll();
            base.Show();
        }

        public override void Hide()
        {
            InputManager.Inputs.PauseUI.Disable();
            base.Hide();
        }

        protected override void OnUIActivated()
        {
            InputManager.Inputs.PauseUI.Enable();
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            gamePausedChannel.RaiseEvent(true);
        }

        protected override void OnUIDeactivated()
        {
            InputManager.Inputs.InGameUI.Enable();
            
            // https://docs.unity3d.com/ScriptReference/Time-timeScale.html WTF?!
            Time.timeScale = previousTimeScale;
            gamePausedChannel.RaiseEvent(false);
        }
        
        void GoToMainMenu()
        {
            OnUIDeactivated();
            sceneLoadChannel.RaiseEvent(SceneLoadOptions.MenuLoad(sceneListSO.mainMenuSceneIndex));
        }

        void DefaultGameInputs.IPauseUIActions.OnResume(InputAction.CallbackContext context)
        {
            if (context.performed == false || isActive == false) return;

            Resume();
        }

        void Resume()
        {
            showPauseUIChannel.RaiseEvent(false);
        }
    }
}