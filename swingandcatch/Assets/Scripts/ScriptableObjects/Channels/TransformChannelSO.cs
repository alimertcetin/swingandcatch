using UnityEngine;

namespace TheGame.ScriptableObjects.Channels
{
    [CreateAssetMenu(menuName = MenuPaths.CHANNEL_BASE_MENU + nameof(TransformChannelSO))]
    public class TransformChannelSO : XIVChannelSO<Transform>
    {
        
    }
}