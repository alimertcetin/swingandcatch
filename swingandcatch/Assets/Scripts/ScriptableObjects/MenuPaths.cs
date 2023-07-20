namespace TheGame.ScriptableObjects
{
    public static class MenuPaths
    {
        public const string BASE_MENU = "TheGame/";

        // All Channels will be under this menu
        public const string CHANNEL_BASE_MENU = "Channels/";
        
        // State Machine
        public const string FSM_BASE = BASE_MENU + "FSM/";
        
        public const string PLAYER_FSM = FSM_BASE + "PlayerFSM/";
        public const string PLAYER_FSM_STATE_DATA = PLAYER_FSM + "State Data/";
        
        public const string SAW_BLADE_FSM = FSM_BASE + "SawBladeFSM/";
        public const string SAW_BLADE_FSM_STATE_DATA = SAW_BLADE_FSM + "State Data/";

        // Scene Management
        public const string SCENE_MANAGEMENT_MENU = BASE_MENU + "Scene Management/";

        // Health System
        public const string HEALTH_SYSTEM_MENU = BASE_MENU + "HealthSystem/";
        
    }
}