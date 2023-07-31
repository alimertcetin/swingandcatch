using TheGame.SceneManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.Scripts.InputSystems;
using TheGame.UISystems.Core;
using TheGame.UISystems.SceneLoading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheGame.UISystems
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] SceneLoadChannelSO displayLoadingScreenChannel;
        [SerializeField] VoidChannelSO stopDisplayingLoadingScreenChannel;
        [SerializeField] BoolChannelSO showPauseUIChannel;

        void OnEnable()
        {
            InputManager.Inputs.InGameUI.Enable();
            displayLoadingScreenChannel.Register(OnDisplayLoadingScreen);
            stopDisplayingLoadingScreenChannel.Register(OnStopDisplayingLoadingScreen);
            showPauseUIChannel.Register(OnShowPauseUI);
            InputManager.Inputs.InGameUI.Pause.performed += OnUIPauseUIPerformed;
        }

        void OnDisable()
        {
            InputManager.Inputs.InGameUI.Disable();
            displayLoadingScreenChannel.Unregister(OnDisplayLoadingScreen);
            stopDisplayingLoadingScreenChannel.Unregister(OnStopDisplayingLoadingScreen);
            showPauseUIChannel.Unregister(OnShowPauseUI);
            InputManager.Inputs.InGameUI.Pause.performed -= OnUIPauseUIPerformed;
        }

        void OnUIPauseUIPerformed(InputAction.CallbackContext obj)
        {
            var pauseUI = UISystem.GetUI<PauseUI>();
            if (pauseUI == null) return;
            showPauseUIChannel.RaiseEvent(!pauseUI.isActive);
        }

        void OnShowPauseUI(bool val)
        {
            // Pause UI raises an event to inform other systems that game is paused or not
            if (val)
            {
                UISystem.Hide<HudUI>();
                UISystem.Show<PauseUI>();
            }
            else
            {
                UISystem.Show<HudUI>();
                UISystem.Hide<PauseUI>();
            }
        }

        void OnDisplayLoadingScreen(SceneLoadOptions sceneLoadOptions)
        {
            var sceneLoadingUI = UISystem.GetUI<SceneLoadingUI>();
            sceneLoadingUI.SetSceneLoadingOptions(sceneLoadOptions);
            sceneLoadingUI.Show();
        }

        void OnStopDisplayingLoadingScreen()
        {
            UISystem.Hide<SceneLoadingUI>();
        }
    }
}