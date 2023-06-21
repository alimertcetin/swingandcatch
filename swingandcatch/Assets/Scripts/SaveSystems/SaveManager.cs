using System;
using System.IO;
using System.Text;
using TheGame.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGame.SaveSystems
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        public bool hasSaveData { get; private set; }
        public int savedSceneIndex { get; private set; } = -1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            void CreateInstance()
            {
                var go = new GameObject("SaveManager");
                Instance = go.AddComponent<SaveManager>();
                DontDestroyOnLoad(go);
            }

            if (Instance == null)
            {
                CreateInstance();
            }
            else
            {
                Destroy(Instance.gameObject);
                CreateInstance();
            }
        }

        void Awake()
        {
            Load();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) return;

            Save();
        }

        void OnApplicationQuit()
        {
            Save();
        }

        void Load()
        {
            hasSaveData = File.Exists(GameData.Save.SaveFilePath);
            if (hasSaveData == false) return;

            savedSceneIndex = Convert.ToInt32(File.ReadAllText(GameData.Save.SaveFilePath));
        }

        void Save()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            savedSceneIndex = sceneIndex;

            if (File.Exists(GameData.Save.SaveFilePath) == false)
            {
                Directory.CreateDirectory(GameData.Save.SaveFolderPath);
                
                var bytes = Encoding.UTF8.GetBytes(sceneIndex.ToString());
                using (FileStream fs = File.Create(GameData.Save.SaveFilePath))
                {
                    fs.Write(bytes);
                }

                return;
            }

            File.WriteAllText(GameData.Save.SaveFilePath, sceneIndex.ToString());
        }
    }
}