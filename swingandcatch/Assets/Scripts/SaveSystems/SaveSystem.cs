using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace TheGame.SaveSystems
{
    public static class SaveSystem
    {
        // static readonly string productName = Application.productName;
        // TODO : we can add slot support by changing the "data" folder name
        public static readonly string saveFolder = Path.Combine(Application.persistentDataPath, "data");
        static readonly Dictionary<string, List<SavableEntity>> savableEntityLookup = new Dictionary<string, List<SavableEntity>>();
        
        public static void Save(string sceneName)
        {
            Directory.CreateDirectory(saveFolder);
            if (TryGetSavableEntitiesInScene(sceneName, out var savableEntities))
            {
                Saver.Save(GetSaveFilePath(sceneName), GetSaveFileBackupPath(sceneName), savableEntities);
            }
        }

        public static void Load(string sceneName)
        {
            Directory.CreateDirectory(saveFolder);
            if (TryGetSavableEntitiesInScene(sceneName, out var savableEntities))
            {
                Saver.Load(GetSaveFilePath(sceneName), GetSaveFileBackupPath(sceneName), savableEntities);
            }
        }

        public static IEnumerator SaveAsync(string sceneName)
        {
            Directory.CreateDirectory(saveFolder);
            if (TryGetSavableEntitiesInScene(sceneName, out var savableEntities))
            {
                yield return AsyncSaver.SaveAsync(GetSaveFilePath(sceneName), GetSaveFileBackupPath(sceneName), savableEntities);
            }
        }

        public static IEnumerator LoadAsync(string sceneName)
        {
            Directory.CreateDirectory(saveFolder);
            if (TryGetSavableEntitiesInScene(sceneName, out var savableEntities))
            {
                yield return AsyncSaver.LoadAsync(GetSaveFilePath(sceneName), GetSaveFileBackupPath(sceneName), savableEntities);
            }
        }

        public static bool IsSaveExists(string sceneName)
        {
            return File.Exists(GetSaveFilePath(sceneName)) || File.Exists(GetSaveFileBackupPath(sceneName));
        }

        public static bool IsSaveExistsAny()
        {
            return Directory.Exists(saveFolder) && Directory.GetFiles(saveFolder).Length > 0;
        }

        public static void ClearSaveData(string sceneName)
        {
            DeleteIfExists(GetSaveFilePath(sceneName));
            DeleteIfExists(GetSaveFileBackupPath(sceneName));

            void DeleteIfExists(string file)
            {
                Task.Factory.StartNew(() =>
                {
                    if (File.Exists(file)) File.Delete(file);
                });
            }
        }

        public static void ClearSaveDataAll()
        {
            if (Directory.Exists(saveFolder) == false) return;
            
            Task.Factory.StartNew(() =>
            {
                var files = Directory.GetFiles(saveFolder);
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }
            });
        }

        public static void AddSavableEntity(string sceneName, SavableEntity entity)
        {
            if (string.IsNullOrWhiteSpace(sceneName)) return;
            if (entity == null) return;
            
            if (savableEntityLookup.ContainsKey(sceneName) == false)
            {
                savableEntityLookup.Add(sceneName, new List<SavableEntity> { entity });
                return;
            }

            var list = savableEntityLookup[sceneName];
            if (list.Contains(entity)) return;
            list.Add(entity);
        }

        public static void RemoveSavableEntity(string sceneName, SavableEntity entity)
        {
            if (string.IsNullOrWhiteSpace(sceneName) || entity == null) return;

            var list = savableEntityLookup[sceneName];
            list.Remove(entity);

            if (list.Count == 0) savableEntityLookup.Remove(sceneName);
        }

        static bool TryGetSavableEntitiesInScene(string sceneName, out SavableEntity[] savableEntities)
        {
            savableEntities = default;
            if (savableEntityLookup.ContainsKey(sceneName) == false) return false;
            savableEntities = savableEntityLookup[sceneName].ToArray();
            return true;
        }

        static string GetSaveFilePath(string sceneName) => Path.Combine(saveFolder, sceneName) + ".sav";
        static string GetSaveFileBackupPath(string sceneName) => Path.Combine(saveFolder, sceneName) + ".bak";
        
    }
}