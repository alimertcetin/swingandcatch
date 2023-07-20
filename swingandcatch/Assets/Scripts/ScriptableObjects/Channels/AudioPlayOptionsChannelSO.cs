using TheGame.AudioManagement;
using UnityEngine;

namespace TheGame.ScriptableObjects.Channels
{
    [CreateAssetMenu(menuName = MenuPaths.CHANNEL_BASE_MENU + nameof(AudioPlayOptionsChannelSO))]
    public class AudioPlayOptionsChannelSO : XIVChannelSO<AudioPlayOptions>
    {
        
    }
}