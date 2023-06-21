using UnityEngine;

namespace TheGame.Data
{
    public static class GameData
    {
        public static class Save
        {
            public const string FILE_NAME = "SAC.sav";
            public const string FOLDER_NAME = "Data";
            public static readonly string SaveFolderPath = System.IO.Path.Combine(Application.persistentDataPath, FOLDER_NAME);
            public static readonly string SaveFilePath = System.IO.Path.Combine(SaveFolderPath, FILE_NAME);
        }
    }
}