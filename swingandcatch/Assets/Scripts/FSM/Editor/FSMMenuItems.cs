using UnityEditor;

namespace TheGame.FSM.Editor
{
    public static class FSMMenuItems
    {
        const string FSM_BASE_MENU = "FSM/";
        const string FSM_DEBUG_MENU = FSM_BASE_MENU + "Debug/";

        [MenuItem(FSM_DEBUG_MENU + nameof(LogStateChanges_TRUE))]
        static void LogStateChanges_TRUE()
        {
            FSMDebugSettings.SetLogStateChanges(true);
        }

        [MenuItem(FSM_DEBUG_MENU + nameof(LogStateChanges_FALSE))]
        static void LogStateChanges_FALSE()
        {
            FSMDebugSettings.SetLogStateChanges(false);
        }
    }
}