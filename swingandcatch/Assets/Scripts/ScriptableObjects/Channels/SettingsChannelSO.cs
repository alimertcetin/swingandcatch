using TheGame.SettingSystems;
using UnityEngine;

namespace TheGame.ScriptableObjects.Channels
{
    [CreateAssetMenu(menuName = MenuPaths.CHANNEL_BASE_MENU + nameof(SettingsChannelSO))]
    public class SettingsChannelSO : XIVChannelSO<Settings>
    {
        
    }
}