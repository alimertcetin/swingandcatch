using TheGame.AudioManagement;
using TheGame.ScriptableObjects.Channels;
using UnityEngine;

namespace TheGame.Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] AudioPlayOptionsChannelSO audioPlayOptionsChannel;
        [SerializeField] AudioClip elevatorMusicClip;

        void Start()
        {
            audioPlayOptionsChannel.RaiseEvent(AudioPlayOptions.MusicPlayOptions(elevatorMusicClip));
        }
    }
}