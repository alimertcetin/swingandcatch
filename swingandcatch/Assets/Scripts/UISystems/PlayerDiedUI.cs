using TheGame.SceneManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TheGame.UISystems
{
    public class PlayerDiedUI : GameUI
    {
        [SerializeField] SceneLoadChannelSO sceneLoadChannel;
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
            sceneLoadChannel.RaiseEvent(new SceneLoadOptions()
            {
                activateImmediately = false,
                displayLoadingScreen = true,
                sceneToLoad = SceneManager.GetActiveScene().buildIndex,
                unloadActiveScene = true,
            });
        }
    }
}