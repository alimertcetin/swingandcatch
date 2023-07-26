using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using TheGame.UISystems.MainMenu;
using UnityEngine;

namespace TheGame.Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] AudioPlayerSO mainMenuMusicAudioPlayer;

        void Start()
        {
            mainMenuMusicAudioPlayer.Play();
            UISystem.Show<MainMenuUI>();
        }

        void OnDestroy()
        {
            mainMenuMusicAudioPlayer.Stop();
        }
    }
}