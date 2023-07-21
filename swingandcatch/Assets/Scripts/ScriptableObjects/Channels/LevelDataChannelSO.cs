using TheGame.SceneManagement;
using UnityEngine;

namespace TheGame.ScriptableObjects.Channels
{
    [CreateAssetMenu(menuName = MenuPaths.CHANNEL_BASE_MENU + nameof(LevelDataChannelSO))]
    public class LevelDataChannelSO : XIVChannelSO<LevelData>
    {
        
    }
}