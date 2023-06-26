using System;
using System.IO;
using TheGame.SaveSystems;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGame.Data.Editor
{
    public static class MenuItems
    {
        const string SAVE_MENU_PATH = "SaveSystem/";

        [MenuItem(SAVE_MENU_PATH + nameof(ClearSaveData))]
        static void ClearSaveData()
        {
            if (Directory.Exists(SaveSystem.saveFolder) == false) return;
            
            var files = Directory.GetFiles(SaveSystem.saveFolder);
            for (var i = 0; i < files.Length; i++)
            {
                string file = files[i];
                File.Delete(file);
            }

            Directory.Delete(SaveSystem.saveFolder);
        }

        [MenuItem(SAVE_MENU_PATH + nameof(OpenSaveDirectory))]
        static void OpenSaveDirectory()
        {
            var dir = Application.persistentDataPath;
            if (Directory.Exists(SaveSystem.saveFolder)) dir = SaveSystem.saveFolder;
            System.Diagnostics.Process.Start(dir);
        }

        [MenuItem(SAVE_MENU_PATH + nameof(ValidateSavablesInTheScene))]
        static void ValidateSavablesInTheScene()
        {
            string error = "";
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                EditorUtility.DisplayProgressBar(nameof(ValidateSavablesInTheScene), "", 0f);
                var scene = SceneManager.GetSceneAt(i);
                var rootGameObjects = scene.GetRootGameObjects();
                var rootGameobjectsLength = rootGameObjects.Length;
                for (var j = 0; j < rootGameobjectsLength; j++)
                {
                    EditorUtility.DisplayProgressBar(nameof(ValidateSavablesInTheScene), $"{j} / {rootGameobjectsLength}", j / (float)rootGameobjectsLength);
                    GameObject rootGameObject = rootGameObjects[j];
                    var childsWithSavable = rootGameObject.GetComponentsInChildren<ISavable>(true);
                    if (rootGameObject.TryGetComponent(out SavableEntity savableEntity) == false && childsWithSavable.Length > 0)
                    {
                        error += $"{rootGameObject} doesn't have {nameof(SavableEntity)} but its children implements {nameof(ISavable) + Environment.NewLine}";
                    }
                    
                }
                EditorUtility.ClearProgressBar();
            }

            if (string.IsNullOrWhiteSpace(error) == false)
            {
                Debug.LogWarning(error);
            }
            
        }
    }
}