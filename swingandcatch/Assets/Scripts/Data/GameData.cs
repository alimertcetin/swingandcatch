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

        public static class SceneData
        {
            // 0 = Initialization, 1 = PersistantManager, 2 = MainMenu
            public const int LEVEL_START_INDEX = 3;
        }
    }
}