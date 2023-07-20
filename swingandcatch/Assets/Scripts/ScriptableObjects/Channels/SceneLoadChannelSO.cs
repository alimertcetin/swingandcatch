using TheGame.SceneManagement;
using UnityEngine;

namespace TheGame.ScriptableObjects.Channels
{
    [CreateAssetMenu(menuName = MenuPaths.CHANNEL_BASE_MENU + nameof(SceneLoadChannelSO))]
    public class SceneLoadChannelSO : XIVChannelSO<SceneLoadOptions>
    {
        
    }
}