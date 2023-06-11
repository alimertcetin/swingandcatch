using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using UnityEngine;

namespace TheGame.UISystems
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] TransformChannelSO playerDiedChannelSO;

        void OnEnable()
        {
            playerDiedChannelSO.Register(OnPlayerDied);
        }

        void OnDisable()
        {
            playerDiedChannelSO.Unregister(OnPlayerDied);
        }

        void OnPlayerDied(Transform playerTransform)
        {
            UISystem.Hide<HudUI>();
            UISystem.Show<PlayerDiedUI>();
        }
    }
}