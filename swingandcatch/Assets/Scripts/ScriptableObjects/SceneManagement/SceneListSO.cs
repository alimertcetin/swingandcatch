using System.Collections.Generic;
using UnityEngine;
using XIV.Core;

namespace TheGame.ScriptableObjects.SceneManagement
{
    [CreateAssetMenu(menuName = MenuPaths.SCENE_MANAGEMENT_MENU + nameof(SceneListSO))]
    public class SceneListSO : ScriptableObject
    {
        [Min(1)] public int persistantManagerSceneIndex;
        [Min(1)] public int mainMenuSceneIndex;

        [HideInInspector] public int lastPlayedLevel;
        [SerializeField] List<int> levelIndices = new List<int>();

        /// <summary>
        /// If false next level is the first level, means we came back to beginning
        /// </summary>
        public bool TryGetNextLevel(int currentLevel, out int nextLevelBuildIndex)
        {
            int index = levelIndices.IndexOf(currentLevel);
            var nextIndex = (index + 1) % levelIndices.Count;
            nextLevelBuildIndex = levelIndices[nextIndex];
            return nextIndex != 0;
        }
        
#if UNITY_EDITOR
        [Button]
        void FillLevelList()
        {
            levelIndices.Clear();
            var scenes = UnityEditor.EditorBuildSettings.scenes;
            
            int length = scenes.Length;
            bool foundLevel = false;
            for (int i = 0; i < length; i++)
            {
                var scene = scenes[i];
                var fullName = scene.path.Split('/')[^1];
                if (fullName.ToLower().Contains("level") == false || fullName.ToLower().Contains("test")) continue;
                foundLevel = true;
                levelIndices.Add(i);
            }

            if (foundLevel == false)
            {
                Debug.LogWarning("Couldn't find any level scene in build settings.");
            }
            
        }
#endif
        
    }
}