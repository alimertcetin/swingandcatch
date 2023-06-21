using UnityEngine;

namespace TheGame.ScriptableObjects.Channels
{
    public struct SceneLoadOptions
    {
        public int sceneToLoad;
        public bool displayLoadingScreen;
        public bool unloadActiveScene;
        public bool activateImmediately;
    }
    
    [CreateAssetMenu(menuName = "Channels/SceneLoadChannelSO")]
    public class SceneLoadChannelSO : XIVChannelSO<SceneLoadOptions>
    {
        
    }
}