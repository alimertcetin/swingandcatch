using TheGame.ScriptableObjects;
using UnityEngine;

namespace TheGame.GraphicManagement
{
    [CreateAssetMenu(menuName = MenuPaths.GRAPHIC_BASE_MENU + nameof(GraphicSettingPresetSO))]
    public class GraphicSettingPresetSO : ScriptableObject
    {
        // All graphic values
        // new SettingParameter(SETTING_PARAMETER_TYPE, PRESET, 0), // Dropdown
        // new SettingParameter(SETTING_PARAMETER_TYPE, RESOLUTION, Vector2Int.zero), // Dropdown
        // new SettingParameter(SETTING_PARAMETER_TYPE, DISPLAY_TYPE, 0), // Dropdown
        // new SettingParameter(SETTING_PARAMETER_TYPE, VSYNC, false), // Toggle
        // new SettingParameter(SETTING_PARAMETER_TYPE, ANTI_ALIAS, 0), // Dropdown
        // new SettingParameter(SETTING_PARAMETER_TYPE, BRIGHTNESS, 0f), // Slider 0-1
        // new SettingParameter(SETTING_PARAMETER_TYPE, SHADOW_QUALITY, 0f), // Dropdown
        // new SettingParameter(SETTING_PARAMETER_TYPE, TEXTURE_QUALITY, 0f), // Dropdown
        
        // The ones we care about
        [Tooltip("0 - Disabled, 1 - 2x MSA, 2 - 4x MSA, 3 - is ignored, 4 - 8x MSA")]
        [Min(0)] public int antiAliasing;
        [Tooltip("0 - Very High, 1 - High, 2 - Medium, 3 - Low, 4 Very Low")]
        [Min(0)] public int shadowQuality;
        [Tooltip("0 - Very High, 1 - High, 2 - Medium, 3 - Low, 4 Very Low")]
        [Min(0)] public int textureQuality;
    }
}