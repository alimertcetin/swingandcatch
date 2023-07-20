using UnityEngine;

namespace TheGame.ScriptableObjects.Channels
{
    [CreateAssetMenu(menuName = MenuPaths.CHANNEL_BASE_MENU + nameof(IntChannelSO))]
    public class IntChannelSO : XIVChannelSO<int>
    {
        
    }
}