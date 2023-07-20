using TheGame.AudioManagement;
using UnityEngine;

namespace TheGame.ScriptableObjects.Channels
{
    [CreateAssetMenu(menuName = MenuPaths.CHANNEL_BASE_MENU + nameof(AudioMixerParameterCollectionChannelSO))]
    public class AudioMixerParameterCollectionChannelSO : XIVChannelSO<AudioMixerParameterCollection>
    {
        
    }
}