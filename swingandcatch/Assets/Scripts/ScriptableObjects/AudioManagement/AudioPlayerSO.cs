using System;
using TheGame.ScriptableObjects.Channels;
using UnityEngine;

namespace TheGame.ScriptableObjects.AudioManagement
{
    [CreateAssetMenu(menuName = MenuPaths.AUDIO_MANAGEMENT_MENU + nameof(AudioPlayerSO))]
    public class AudioPlayerSO : ScriptableObject
    {
        [SerializeField] AudioPlayOptionsChannelSO audioPlayOptionsChannel;
        [SerializeField] AudioClip clip;
        [SerializeField] AudioType audioType;

        public void Play()
        {
            audioPlayOptionsChannel.RaiseEvent(GetOption());
        }

        public void Stop()
        {
            audioPlayOptionsChannel.RaiseEvent(new AudioPlayOptions(audioType, null, false, null));
        }

        AudioPlayOptions GetOption()
        {
            return audioType switch
            {
                AudioType.None => default,
                AudioType.Music => AudioPlayOptions.MusicPlayOptions(clip),
                AudioType.Effect => AudioPlayOptions.EffectPlayOptions(clip),
                _ => throw new NotImplementedException(audioType + " is not implemented.")
            };
        }
    }
}