using System;
using System.Collections.Generic;
using TheGame.SceneManagement;
using UnityEngine;
using XIV.Core;

namespace TheGame.ScriptableObjects.SceneManagement
{
    [CreateAssetMenu(menuName = MenuPaths.SCENE_MANAGEMENT_MENU + nameof(SceneListSO))]
    public class SceneListSO : ScriptableObject
    {
        public int persistantManagerSceneIndex = 1;
        public int mainMenuSceneIndex = 2;
        [SerializeField] List<int> levelIndices = new List<int>();
        
        public LevelData GetSceneListData() => new LevelData(levelIndices);
        
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