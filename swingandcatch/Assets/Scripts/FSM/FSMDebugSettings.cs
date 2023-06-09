#if UNITY_EDITOR
using UnityEditor;

namespace TheGame.FSM
{
    public static class FSMDebugSettings
    {
        const string LOG_STATE_CHANGES_KEY = "Log_State_Changes";
        
        public static bool IsStateChangeLogsEnabled => EditorPrefs.GetBool(LOG_STATE_CHANGES_KEY);

        public static void SetLogStateChanges(bool val)
        {
            EditorPrefs.SetBool(LOG_STATE_CHANGES_KEY, val);
        }
    }
}
#endif