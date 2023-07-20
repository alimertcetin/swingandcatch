using TheGame.ScriptableObjects.Channels;
using UnityEngine;

namespace TheGame.Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] AudioPlayerSO mainMenuMusicAudioPlayer;

        void Start()
        {
            mainMenuMusicAudioPlayer.Play();
        }

        void OnDestroy()
        {
            mainMenuMusicAudioPlayer.Stop();
        }
    }
}