using TheGame.PlayerSystems;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using UnityEngine;

namespace TheGame.UISystems
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] TransformChannelSO playerDiedChannelSO;
        [SerializeField] TransformChannelSO playerReachedEndChannelSO;
        [SerializeField] BoolChannelSO displayLoadingScreenChannel;

        void OnEnable()
        {
            playerDiedChannelSO.Register(OnPlayerDied);
            playerReachedEndChannelSO.Register(OnPlayerReachedEnd);
            displayLoadingScreenChannel.Register(OnDisplayLoadingScreen);
        }

        void OnDisable()
        {
            playerDiedChannelSO.Unregister(OnPlayerDied);
            playerReachedEndChannelSO.Unregister(OnPlayerReachedEnd);
            displayLoadingScreenChannel.Unregister(OnDisplayLoadingScreen);
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

        void OnDisplayLoadingScreen(bool value)
        {
            if (value) UISystem.Show<SceneLoadingUI>();
            else UISystem.Hide<SceneLoadingUI>();
        }
    }
}