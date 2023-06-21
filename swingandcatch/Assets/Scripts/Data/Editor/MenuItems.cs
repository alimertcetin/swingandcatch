using System.IO;
using UnityEditor;

namespace TheGame.Data.Editor
{
    public static class MenuItems
    {
        const string SAVE_MENU_PATH = "SaveSystem/";

        [MenuItem(SAVE_MENU_PATH + nameof(ClearSaveData))]
        static void ClearSaveData()
        {
            if (File.Exists(GameData.Save.SaveFilePath))
            {
                File.Delete(GameData.Save.SaveFilePath);
            }
        }
    }
}