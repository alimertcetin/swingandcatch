using TheGame.SceneManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using TheGame.UISystems.SceneLoading;
using UnityEngine;

namespace TheGame.UISystems
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] TransformChannelSO playerDiedChannelSO;
        [SerializeField] VoidChannelSO showWinUIChannel;
        [SerializeField] SceneLoadChannelSO displayLoadingScreenChannel;
        [SerializeField] VoidChannelSO stopDisplayingLoadingScreenChannel;

        void OnEnable()
        {
            playerDiedChannelSO.Register(OnPlayerDied);
            showWinUIChannel.Register(ShowWinUI);
            displayLoadingScreenChannel.Register(OnDisplayLoadingScreen);
            stopDisplayingLoadingScreenChannel.Register(OnStopDisplayingLoadingScreen);
        }

        void OnDisable()
        {
            playerDiedChannelSO.Unregister(OnPlayerDied);
            showWinUIChannel.Unregister(ShowWinUI);
            displayLoadingScreenChannel.Unregister(OnDisplayLoadingScreen);
            stopDisplayingLoadingScreenChannel.Unregister(OnStopDisplayingLoadingScreen);
        }

        void OnPlayerDied(Transform playerTransform)
        {
            UISystem.Hide<HudUI>();
            UISystem.Show<PlayerDiedUI>();
        }

        void ShowWinUI()
        {
            UISystem.Hide<HudUI>();
            UISystem.Show<PlayerWinUI>();
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