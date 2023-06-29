using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using XIV.XIVEditor.Utils;

namespace LessonIsMath.XIVEditor.Windows
{
    public class EasySceneLoaderWindow : EditorWindow
    {
        bool isInitialized;
        List<SceneAsset> scenes = new List<SceneAsset>();
        List<SceneAsset> testScenes = new List<SceneAsset>();
        Vector2 scenesScrollPos;
        Vector2 buttonsScrollPos;
        bool additiveLoadToggle;
        Color additiveLoadToggleColor => additiveLoadToggle ? Color.white : Color.gray;

        string sceneFolder;
        string testFolder;
        string searchStr;

        string sceneFolderKey => nameof(EasySceneLoaderWindow) + "_SceneFolderPath";
        string testFolderKey => nameof(EasySceneLoaderWindow) + "_TestFolderPath";

        GUIStyle labelStyle;
        bool isSearching;
        
        [MenuItem("TheGame/Utilities/" + nameof(EasySceneLoaderWindow))]
        public static void ShowSceneLoaderWindow()
        {
            EditorWindow.GetWindow<EasySceneLoaderWindow>("Easy Scene Loader").Show();
        }

        void OnEnable()
        {
            sceneFolder = EditorPrefs.GetString(sceneFolderKey, "Assets/Scenes");
            testFolder = EditorPrefs.GetString(testFolderKey, "Assets/Tests");
        }

        void Initialize()
        {
            AddScenes(sceneFolder, scenes);
            AddScenes(testFolder, testScenes);
            labelStyle = EditorStyles.boldLabel;
            labelStyle.alignment = TextAnchor.MiddleCenter;
        }

        void OnProjectChange()
        {
            scenes.Clear();
            testScenes.Clear();
            AddScenes(sceneFolder, scenes);
            AddScenes(testFolder, testScenes);
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            EditorPrefs.SetString(sceneFolderKey, sceneFolder);
            EditorPrefs.SetString(testFolderKey, testFolder);
        }

        void OnDestroy()
        {
            SaveChanges();
        }

        void OnGUI()
        {
            if (isInitialized == false)
            {
                Initialize();
                isInitialized = true;
            }

            Color tempGUIColor = GUI.color;
            Color labelColor = Color.green;

            GUILayout.Space(20f);
            buttonsScrollPos = EditorGUILayout.BeginScrollView(buttonsScrollPos, false, false, 
                GUI.skin.horizontalScrollbar, GUIStyle.none, GUI.skin.scrollView);
            buttonsScrollPos.y = 0f;
            EditorGUILayout.BeginHorizontal();
            GUI.color = additiveLoadToggleColor;
            additiveLoadToggle = GUILayout.Toggle(additiveLoadToggle, "Load additive");
            GUI.color = tempGUIColor;
            if (GUILayout.Button("Select scene folder")) EditorUtils.HighlightOrCreateFolder(sceneFolder);
            if (GUILayout.Button("Select test folder")) EditorUtils.HighlightOrCreateFolder(testFolder);
            if (GUILayout.Button("Refresh")) OnProjectChange();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20f);
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scene Folder Path");
            sceneFolder = EditorGUILayout.TextField(sceneFolder);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Test Folder Path");
            testFolder = EditorGUILayout.TextField(testFolder);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Search");
            searchStr = EditorGUILayout.TextField(searchStr);
            isSearching = string.IsNullOrWhiteSpace(searchStr) == false;
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(20f);
            
            scenesScrollPos = EditorGUILayout.BeginScrollView(scenesScrollPos, false, false);

            if (isSearching)
            {
                var allScenes = UnityEngine.Pool.ListPool<SceneAsset>.Get();
                allScenes.AddRange(scenes);
                allScenes.AddRange(testScenes);
                int count = allScenes.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    var sceneAsset = allScenes[i];
                    if (sceneAsset.name.ToLower().Contains(searchStr.ToLower()) == false)
                    {
                        allScenes.RemoveAt(i);
                        count--;
                    }
                }
                
                for (var i = 0; i < count; i++)
                {
                    SceneAsset sceneAsset = allScenes[i];
                    GUILayout.Space(10);
                    if (GUILayout.Button(sceneAsset.name, GUILayout.Height(50)) == false) continue;

                    LoadScene(sceneAsset, additiveLoadToggle);
                }
                UnityEngine.Pool.ListPool<SceneAsset>.Release(allScenes);
            }
            else
            {
                GUI.color = labelColor;
                if (scenes.Count > 0) GUILayout.Label("Game Scenes", labelStyle);
                GUI.color = tempGUIColor;

                for (var i = 0; i < scenes.Count; i++)
                {
                    SceneAsset sceneAsset = scenes[i];
                    GUILayout.Space(10);
                    if (GUILayout.Button(sceneAsset.name, GUILayout.Height(50)) == false) continue;

                    LoadScene(sceneAsset, additiveLoadToggle);
                }

                GUILayout.Space(20f);

                GUI.color = labelColor;
                if (testScenes.Count > 0) GUILayout.Label("Test Scenes", labelStyle);
                GUI.color = tempGUIColor;

                for (var i = 0; i < testScenes.Count; i++)
                {
                    SceneAsset sceneAsset = testScenes[i];
                    GUILayout.Space(10);
                    if (GUILayout.Button(sceneAsset.name, GUILayout.Height(50)) == false) continue;

                    LoadScene(sceneAsset, additiveLoadToggle);
                }
            }

            EditorGUILayout.EndScrollView();
            if (Event.current.isScrollWheel && Event.current.type != EventType.Used)
            {
                float scrollAmount = Event.current.delta.y * 10f;
                float horizontalScrollPosition = buttonsScrollPos.x + scrollAmount;
                buttonsScrollPos = new Vector2(horizontalScrollPosition, buttonsScrollPos.y);
                buttonsScrollPos.x = buttonsScrollPos.x < 0 ? 0 : buttonsScrollPos.x;
                Event.current.Use();
            }
        }

        static void LoadScene(SceneAsset sceneAsset, bool additive)
        {
            int countLoaded = SceneManager.sceneCount;
            Scene[] loadedScenes = new Scene[countLoaded];
            for (int i = 0; i < countLoaded; i++)
            {
                loadedScenes[i] = SceneManager.GetSceneAt(i);
            }

            if (additive)
            {
                if (Application.isPlaying) SceneManager.LoadScene(sceneAsset.name, LoadSceneMode.Additive);
                else EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneAsset), OpenSceneMode.Additive);
                return;
            }
            // returns false if canceled
            bool shouldLoad = EditorSceneManager.SaveModifiedScenesIfUserWantsTo(loadedScenes);
            if (shouldLoad == false) return;
            if (Application.isPlaying) SceneManager.LoadScene(sceneAsset.name);
            else EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneAsset));
        }

        void AddScenes(string path, List<SceneAsset> collection)
        {
            if (System.IO.Directory.Exists(path) == false) return;
            var valueCollection = AssetUtils.LoadAssetsOfType<SceneAsset>(path);
            collection.AddRange(valueCollection);
        }
    }
}