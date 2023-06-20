using TheGame.PlayerSystems;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using UnityEngine;

namespace TheGame.UISystems
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] TransformChannelSO playerDiedChannelSO;
        [SerializeField] TransformChannelSO playerUpdateHealthChannelSO;
        [SerializeField] TransformChannelSO playerReachedEndChannelSO;

        void OnEnable()
        {
            playerUpdateHealthChannelSO.Register(OnPlayerRecievedDamage);
            playerDiedChannelSO.Register(OnPlayerDied);
            playerReachedEndChannelSO.Register(OnPlayerReachedEnd);
        }

        void OnDisable()
        {
            playerUpdateHealthChannelSO.Unregister(OnPlayerRecievedDamage);
            playerDiedChannelSO.Unregister(OnPlayerDied);
            playerReachedEndChannelSO.Unregister(OnPlayerReachedEnd);
        }

        void OnPlayerRecievedDamage(Transform playerTransform)
        {
            var currentHealth = playerTransform.GetComponent<PlayerFSM>().health;
            UISystem.GetUI<HudUI>().healthPageUI.ChangeDisplayAmount(currentHealth / 100f);
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
    }
}