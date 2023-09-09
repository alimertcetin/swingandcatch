namespace XIV_Packages.PCSettingSystems.Core
{
    public interface ISetting
    {
        /// <summary>
        /// Presets will ignore this setting if this returns false
        /// </summary>
        bool canIncludeInPresets { get; }
        bool IsCritical { get; }
    }
}