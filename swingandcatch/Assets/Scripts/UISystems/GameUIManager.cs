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

        void OnEnable()
        {
            playerDiedChannelSO.Register(OnPlayerDied);
            playerUpdateHealthChannelSO.Register(OnPlayerRecievedDamage);
        }

        void OnDisable()
        {
            playerDiedChannelSO.Unregister(OnPlayerDied);
            playerUpdateHealthChannelSO.Unregister(OnPlayerRecievedDamage);
        }

        void OnPlayerDied(Transform playerTransform)
        {
            UISystem.Hide<HudUI>();
            UISystem.Show<PlayerDiedUI>();
        }

        void OnPlayerRecievedDamage(Transform playerTransform)
        {
            var currentHealth = playerTransform.GetComponent<PlayerFSM>().health;
            UISystem.GetUI<HudUI>().healthPageUI.ChangeDisplayAmount(currentHealth / 100f);
        }
    }
}