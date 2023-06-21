using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using XIV.Core;

namespace TheGame.ScriptableObjects
{
    [CreateAssetMenu(menuName = MenuPaths.SCENE_MANAGEMENT_MENU + nameof(LevelListSO))]
    public class LevelListSO : ScriptableObject
    {
        [Header("Build indices of Level Scenes")]
        public List<int> buildIndices = new();
#if UNITY_EDITOR
        [Button]
        void FillSceneList()
        {
            buildIndices.Clear();
            var scenes = UnityEditor.EditorBuildSettings.scenes;
            
            int length = scenes.Length;
            bool foundLevel = false;
            for (int i = 0; i < length; i++)
            {
                var scene = scenes[i];
                var fullName = scene.path.Split('/')[^1];
                if (fullName.ToLower().Contains("level") == false || fullName.ToLower().Contains("test")) continue;
                foundLevel = true;
                buildIndices.Add(i);
            }

            if (foundLevel == false)
            {
                Debug.LogWarning("Couldn't find any level scene in build settings.");
            }
            
        }
#endif
        
    }
}
