using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using XIV.XIVEditor.Utils;

namespace TheGame.Editor
{
    public class EasySceneLoaderWindow : EditorWindow
    {
        bool isInitialized;
        
        Vector2 scenesScrollPos;
        Vector2 buttonsScrollPos;
        
        bool additiveLoadToggle;
        Color additiveLoadToggleColor => additiveLoadToggle ? Color.white : Color.gray;

        readonly Dictionary<string, List<SceneAsset>> scenes = new Dictionary<string, List<SceneAsset>>();

        const string sceneFolderPathSearchLiteral = nameof(EasySceneLoaderWindow) + "_SceneFolder_{0}";
        const string sceneFolderCountKey = nameof(EasySceneLoaderWindow) + "_SceneFolderCount";
        int sceneFolderCount;

        GUIStyle labelStyle;
        
        string searchStr;
        bool isSearching;

        readonly List<string> filteredFolderDisplayList = new List<string>();

        Color settingsButtonColor;
        Color filteredDisplayColor;
        
        [MenuItem("TheGame/Utilities/" + nameof(EasySceneLoaderWindow))]
        public static void ShowSceneLoaderWindow()
        {
            GetWindow<EasySceneLoaderWindow>("Easy Scene Loader").Show();
        }

        void OnEnable()
        {
            isInitialized = false;
            ColorUtility.TryParseHtmlString("#F3AA60", out settingsButtonColor);
            ColorUtility.TryParseHtmlString("#EF6262", out filteredDisplayColor);
        }

        void Initialize()
        {
            sceneFolderCount = EditorPrefs.GetInt(sceneFolderCountKey, 0);
            AddAllScenes();
            labelStyle = EditorStyles.boldLabel;
            labelStyle.alignment = TextAnchor.MiddleCenter;
        }

        void OnProjectChange()
        {
            SaveChanges();
            AddAllScenes();
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            int keysCount = scenes.Keys.Count;
            EditorPrefs.SetInt(sceneFolderCountKey, keysCount);
            sceneFolderCount = keysCount;
            int index = 0;
            foreach (string scenesKey in scenes.Keys)
            {
                EditorPrefs.SetString(string.Format(sceneFolderPathSearchLiteral, index++), scenesKey);
            }
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

            HandleDragAndDrops();

            EditorGUILayout.Space(20f);
            buttonsScrollPos = EditorGUILayout.BeginScrollView(buttonsScrollPos, false, false, GUILayout.ExpandHeight(false));
            buttonsScrollPos.y = 0f;
            
            EditorGUILayout.BeginHorizontal();

            if (scenes.Count > 0) DrawLabel("Manage : ");
            foreach (string folderPath in scenes.Keys)
            {
                var folderName = folderPath.Split('/')[^1];
                bool isFilteredDisplaying = filteredFolderDisplayList.Contains(folderPath);
                
                if (DrawButton(folderName, isFilteredDisplaying ? filteredDisplayColor : default) == false) continue;
                
                // Display Solo if clicked
                if (Event.current.button == 0) // 0 -> LMB, 1 -> RMB
                {
                    if (isFilteredDisplaying) filteredFolderDisplayList.Remove(folderPath);
                    else filteredFolderDisplayList.Add(folderPath);
                    continue;
                }

                // Show Context menu if right clicked
                var genericMenu = new GenericMenu();
                genericMenu.AddItem(new GUIContent("Highlight Folder"), false, () => EditorUtils.Highlight(folderPath));
                genericMenu.AddItem(new GUIContent("Display Solo"), isFilteredDisplaying, () =>
                {
                    if (isFilteredDisplaying)
                    {
                        filteredFolderDisplayList.Remove(folderPath);
                        return;
                    }
                    filteredFolderDisplayList.Add(folderPath);
                });
                genericMenu.AddItem(new GUIContent("Remove Folder"), false, () =>
                {
                    var title = "Remove Selected Folder?";
                    var message = "You can always re-add the folder by drag and drop";
                    var result = EditorUtility.DisplayDialog(title, message, "Yes", "Cancel");
                    if (!result) return;
                    RemoveFolder(folderPath);
                    if (isFilteredDisplaying) filteredFolderDisplayList.Remove(folderPath);
                });
                genericMenu.ShowAsContext();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndScrollView();

            if (scenes.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Search : ");
                searchStr = EditorGUILayout.TextField(searchStr);
                isSearching = string.IsNullOrWhiteSpace(searchStr) == false;
                EditorGUILayout.EndHorizontal();
            }
            
            GUILayout.Space(10f);
            if (scenes.Count > 0)
            {
                additiveLoadToggle = DrawToggle("Load additive", additiveLoadToggle, additiveLoadToggleColor);
                
                if (DrawButton("Settings", settingsButtonColor))
                {
                    var genericMenu = new GenericMenu();
                    genericMenu.AddItem(new GUIContent("Refresh Scene List"), false, OnProjectChange);
                    genericMenu.AddItem(new GUIContent("Clear Filter"), false, filteredFolderDisplayList.Clear);
                    genericMenu.ShowAsContext();
                }
            }
            GUILayout.Space(10f);

            scenesScrollPos = EditorGUILayout.BeginScrollView(scenesScrollPos, false, false, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            if (filteredFolderDisplayList.Count > 0)
            {
                int filteredDisplayCount = filteredFolderDisplayList.Count;
                for (int i = 0; i < filteredDisplayCount; i++)
                {
                    var displayFolder = filteredFolderDisplayList[i];
                    if (i > 0) GUILayout.Space(10);
                    DrawLabel("Filtered : " + displayFolder.Split('/')[^1], labelStyle, filteredDisplayColor);
                    DisplayScenes(scenes[displayFolder]);
                }
            }
            else
            {
                DisplayAllScenes();
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

        void HandleDragAndDrops()
        {
            if (scenes.Count == 0) DrawLabel("Drop folders to track scenes", labelStyle);
            var addDropObjects = EditorUtils.HandleDragAndDrop();
            if (addDropObjects != null)
            {
                foreach (object o in addDropObjects)
                {
                    if (o is SceneAsset || o is DefaultAsset)
                    {
                        AddFolder(AssetDatabase.GetAssetPath(o as Object));
                    }
                }
            }
        }

        bool DrawButton(string text, Color c = default)
        {
            var prevCol = GUI.color;
            GUI.color = c == default ? prevCol : c;
            bool val = GUILayout.Button(text);
            GUI.color = prevCol;
            return val;
        }

        void DisplayAllScenes()
        {
            if (isSearching)
            {
                var filteredScenes = ListPool<SceneAsset>.Get();
                foreach (List<SceneAsset> sceneAssets in scenes.Values)
                {
                    filteredScenes.AddRange(sceneAssets);
                }

                // Remove non-matching names from scene list
                int count = filteredScenes.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (filteredScenes[i].name.ToLower(CultureInfo.InvariantCulture).Contains(searchStr.ToLower(CultureInfo.InvariantCulture)) == false)
                    {
                        filteredScenes.RemoveAt(i);
                    }
                }

                // Display current matching scenes
                DisplayScenes(filteredScenes);

                ListPool<SceneAsset>.Release(filteredScenes);
            }
            else
            {
                foreach (string folderPath in scenes.Keys)
                {
                    var folderName = folderPath.Split('/')[^1];
                    DrawLabel(folderName, labelStyle, Color.green);
                    DisplayScenes(scenes[folderPath]);
                    GUILayout.Space(20f);
                }
            }
        }

        void DisplayScenes(List<SceneAsset> sceneList)
        {
            ColorUtility.TryParseHtmlString("#D8D9DA", out var c1);
            ColorUtility.TryParseHtmlString("#FFF6E0", out var c2);
            var tempColor = GUI.backgroundColor;
            int sceneListCount = sceneList.Count;
            for (int i = 0; i < sceneListCount; i++)
            {
                SceneAsset sceneAsset = sceneList[i];
                GUILayout.Space(10);
                GUI.backgroundColor = i % 2 == 0 ? c1 : c2;
                if (GUILayout.Button(sceneAsset.name, GUILayout.Height(50)) == false) continue;

                LoadScene(sceneAsset, additiveLoadToggle);
            }

            GUI.backgroundColor = tempColor;
        }

        static bool DrawToggle(string text, bool value, Color color = default)
        {
            var prevColor = GUI.color;
            color = color == default ? prevColor : color;
            
            GUI.color = color;
            value = GUILayout.Toggle(value, text);
            GUI.color = prevColor;
            return value;
        }

        static void DrawLabel(string text, GUIStyle labelStyle = default, Color color = default)
        {
            labelStyle ??= EditorStyles.label;
            var prevColor = GUI.color;
            color = color == default ? prevColor : color;
            GUI.color = color;
            GUILayout.Label(text, labelStyle);
            GUI.color = prevColor;
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

        void AddAllScenes()
        {
            scenes.Clear();
            for (int i = 0; i < sceneFolderCount; i++)
            {
                var key = string.Format(sceneFolderPathSearchLiteral, i);
                var scenePath = EditorPrefs.GetString(key, null);
                if (string.IsNullOrWhiteSpace(scenePath)) continue;
                List<SceneAsset> sceneAssets = new List<SceneAsset>();
                AddScenesToCollection(scenePath, sceneAssets);
                if (sceneAssets.Count == 0)
                {
                    // key exists but there is no scene in the folder anymore
                    EditorPrefs.DeleteKey(key);
                    continue;
                }
                scenes.Add(scenePath, sceneAssets);
            }
        }

        void AddScenesToCollection(string path, List<SceneAsset> collection)
        {
            if (System.IO.Directory.Exists(path) == false) return;
            var sceneAssets = AssetUtils.LoadAssetsOfType<SceneAsset>(path);
            collection.AddRange(sceneAssets);
        }

        void DisplaySolo(string folderPath)
        {
            filteredFolderDisplayList.Add(folderPath);
        }

        void AddFolder(string folderPath)
        {
            if (scenes.ContainsKey(folderPath))
            {
                var paths = scenes[folderPath];
                paths.Clear();
                AddScenesToCollection(folderPath, paths);
                if (paths.Count > 0) OnProjectChange();
                return;
            }
            var list = new List<SceneAsset>();
            AddScenesToCollection(folderPath, list);
            if (list.Count > 0)
            {
                scenes.Add(folderPath, list);
                OnProjectChange();
            }
        }

        void RemoveFolder(string folderPath)
        {
            scenes.Remove(folderPath);
            OnProjectChange();
        }
    }
}