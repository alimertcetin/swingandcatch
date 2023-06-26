using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TheGame.SaveSystems
{
    public static class AsyncSaver
    {
        static readonly StringBuilder stringBuilder = new StringBuilder(1000);

        public static IEnumerator SaveAsync(string saveFile, string saveFileBackup, SavableEntity[] savableEntities)
        {
            yield return LoadFromFileAsync(saveFile);
            
            Dictionary<string, object> deserializedData;
            if (stringBuilder.Length != 0)
            {
                // We loaded the data from the original file, Create a backup of it
                var data = stringBuilder.ToString();
                deserializedData = XIVSerializer.Deserialize<Dictionary<string, object>>(data);
                yield return SaveToFileAsync(saveFileBackup, deserializedData);
            }
            else
            {
                // Couldnt load the data from original file, try backup file
                yield return LoadFromFileAsync(saveFileBackup);

                if (stringBuilder.Length != 0)
                {
                    var data = stringBuilder.ToString();
                    deserializedData = XIVSerializer.Deserialize<Dictionary<string, object>>(data);
                }
                else
                {
                    // Couldnt load the data from backup file, assume there is no save file
                    deserializedData = new Dictionary<string, object>();
                }
            }

            yield return GetSaveDataFromSavablesAsync(deserializedData, savableEntities);
            
            var saveTask = SaveToFileAsync(saveFile, deserializedData);
            // saveTask.Start();
            while (saveTask.IsCompleted == false)
            {
                yield return null;
            }
        }

        public static IEnumerator LoadAsync(string saveFile, string saveFileBackup, SavableEntity[] savableEntities)
        {
            yield return LoadFromFileAsync(saveFile);
            if (stringBuilder.Length == 0) yield return LoadFromFileAsync(saveFileBackup);
            var deserializedData = stringBuilder.Length > 0 ? XIVSerializer.Deserialize<Dictionary<string, object>>(stringBuilder.ToString()) : default;
            
            if (deserializedData == default) yield break;
            yield return LoadSaveDataToSavablesAsync(deserializedData, savableEntities);
        }

        static async Task SaveToFileAsync(string saveFile, object deserializedData)
        {
            using (var saveFileStream = File.Open(saveFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter(saveFileStream))
                {
                    string serializedData = XIVSerializer.Serialize(deserializedData);
                    await streamWriter.WriteAsync(serializedData);
                }
            }
        }

        static IEnumerator LoadFromFileAsync(string saveFile)
        {
            stringBuilder.Clear();
            if (File.Exists(saveFile) == false) yield break;
            
            const int BATCH_SIZE = 50;
            int current = 0;
            using (FileStream fileStream = File.Open(saveFile, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    while (streamReader.EndOfStream == false)
                    {
                        stringBuilder.AppendLine(streamReader.ReadLine());
                        current++;
                        if (current == BATCH_SIZE)
                        {
                            current = 0;
                            yield return null;
                        }
                    }
                }
            }
        }

        static IEnumerator GetSaveDataFromSavablesAsync(Dictionary<string, object> deserializedData, SavableEntity[] savableEntities)
        {
            const int BATCH_SIZE = 5;
            int current = 0;
            for (var i = 0; i < savableEntities.Length; i++, current++)
            {
                if (current == BATCH_SIZE)
                {
                    current = 0;
                    yield return null;
                }
                var savableEntity = savableEntities[i];
                var savableEntityState = savableEntity.GetSaveData();
                if (deserializedData.ContainsKey(savableEntity.guid)) deserializedData[savableEntity.guid] = savableEntityState;
                else deserializedData.Add(savableEntity.guid, savableEntityState);
            }
        }

        static IEnumerator LoadSaveDataToSavablesAsync(Dictionary<string, object> deserializedData, SavableEntity[] savableEntities)
        {
            const int BATCH_SIZE = 5;
            int current = 0;
            for (var i = 0; i < savableEntities.Length; i++, current++)
            {
                if (current == BATCH_SIZE)
                {
                    current = 0;
                    yield return null;
                }
                var savable = savableEntities[i];
                if (deserializedData.TryGetValue(savable.guid, out object value))
                {
                    savable.LoadSaveData(value);
                }
            }
        }
    }
}