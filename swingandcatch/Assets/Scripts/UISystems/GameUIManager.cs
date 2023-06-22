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
        [SerializeField] TransformChannelSO playerReachedEndChannelSO;
        [SerializeField] SceneLoadChannelSO displayLoadingScreenChannel;
        [SerializeField] VoidChannelSO stopDisplayingLoadingScreenChannel;

        void OnEnable()
        {
            playerDiedChannelSO.Register(OnPlayerDied);
            playerReachedEndChannelSO.Register(OnPlayerReachedEnd);
            displayLoadingScreenChannel.Register(OnDisplayLoadingScreen);
            stopDisplayingLoadingScreenChannel.Register(OnStopDisplayingLoadingScreen);
        }

        void OnDisable()
        {
            playerDiedChannelSO.Unregister(OnPlayerDied);
            playerReachedEndChannelSO.Unregister(OnPlayerReachedEnd);
            displayLoadingScreenChannel.Unregister(OnDisplayLoadingScreen);
            stopDisplayingLoadingScreenChannel.Unregister(OnStopDisplayingLoadingScreen);
        }

        void OnPlayerDied(Transform playerTransform)
        {
            UISystem.Hide<HudUI>();
            UISystem.Show<PlayerDiedUI>();
        }

        void OnPlayerReachedEnd(Transform playerTransform)
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