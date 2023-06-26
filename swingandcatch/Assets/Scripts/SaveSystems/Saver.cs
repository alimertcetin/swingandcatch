using System.Collections.Generic;
using System.IO;

namespace TheGame.SaveSystems
{
    public static class Saver
    {
        public static void Save(string saveFile, string saveFileBackup, SavableEntity[] savableEntities)
        {
            var state = LoadFromFile(saveFile) ?? LoadFromFile(saveFileBackup) ?? new Dictionary<string, object>();
            GetSaveDataFromSavables(state, savableEntities);
            SaveToFile(saveFile, state);
        }

        public static void Load(string saveFile, string saveFileBackup, SavableEntity[] savableEntities)
        {
            var state = LoadFromFile(saveFile) ?? LoadFromFile(saveFileBackup);
            
            // Couldnt load data from files, assume there is no file
            if (state == default) return;

            LoadSaveDataToSavables(state, savableEntities);
        }

        static void SaveToFile(string saveFile, object deserializedData)
        {
            using (var saveFileStream = File.Open(saveFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter(saveFileStream))
                {
                    string serializedData = XIVSerializer.Serialize(deserializedData);
                    streamWriter.Write(serializedData);
                }
            }
        }

        static Dictionary<string, object> LoadFromFile(string saveFile)
        {
            if (File.Exists(saveFile) == false) return default;
            
            using (FileStream fileStream = File.Open(saveFile, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    return XIVSerializer.Deserialize<Dictionary<string, object>>(streamReader.ReadToEnd());
                }
            }
        }

        static void GetSaveDataFromSavables(Dictionary<string, object> currentData, SavableEntity[] savableEntities)
        {
            for (var i = 0; i < savableEntities.Length; i++)
            {
                var savableEntity = savableEntities[i];
                var savableEntitySaveData = savableEntity.GetSaveData();

                if (currentData.ContainsKey(savableEntity.guid)) currentData[savableEntity.guid] = savableEntitySaveData;
                else currentData.Add(savableEntity.guid, savableEntitySaveData);
            }
        }

        static void LoadSaveDataToSavables(Dictionary<string, object> data, SavableEntity[] savableEntities)
        {
            for (var i = 0; i < savableEntities.Length; i++)
            {
                var savableEntity = savableEntities[i];
                if (data.TryGetValue(savableEntity.guid, out object value))
                {
                    savableEntity.LoadSaveData(value);
                }
            }
        }
    }
}