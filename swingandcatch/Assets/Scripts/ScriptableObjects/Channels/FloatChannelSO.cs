using UnityEngine;

namespace TheGame.ScriptableObjects.Channels
{
    [CreateAssetMenu(menuName = MenuPaths.CHANNEL_BASE_MENU + nameof(FloatChannelSO))]
    public class FloatChannelSO : XIVChannelSO<float>
    {
        
    }
}