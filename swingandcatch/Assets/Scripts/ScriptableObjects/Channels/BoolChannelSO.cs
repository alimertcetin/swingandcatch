using UnityEngine;

namespace TheGame.ScriptableObjects.Channels
{
    [CreateAssetMenu(menuName = MenuPaths.CHANNEL_BASE_MENU + nameof(BoolChannelSO))]
    public class BoolChannelSO : XIVChannelSO<bool>
    {
        
    }
}