using TheGame.SaveSystems;
using UnityEngine;
using XIV_Packages.PCSettingSystems.Core;
using XIV_Packages.PCSettingSystems.Extras.ScriptableObjects;
using XIV_Packages.PCSettingSystems.Extras.SettingAppliers;
using XIV_Packages.PCSettingSystems.Extras.SettingApplyCommands.GraphicCommands;
using XIV_Packages.PCSettingSystems.Extras.SettingContainers;

namespace XIV_Packages.PCSettingSystems.Extras
{
    public class GraphicSettingManager : SettingManager, ISavable
    {
        [SerializeField] GraphicPresetItemSO[] presets;
        [SerializeField] int defaultPresetIndex;

        GraphicSettingContainer graphicSettingContainer;

        public override void InitializeContainer()
        {
            graphicSettingContainer = new GraphicSettingContainer(CreateGraphicSettingApplier());
            SettingPreset[] graphicPresets = CreateSettingPresets();
            graphicSettingContainer.AddPresets(graphicPresets);
            graphicSettingContainer.InitializeSettings(graphicPresets[defaultPresetIndex]);
            graphicSettingContainer.ApplyChanges();
            graphicSettingContainer.ClearUndoHistory();
        }

        public override ISettingContainer GetContainer() => graphicSettingContainer;

        static ISettingApplier CreateGraphicSettingApplier()
        {
            ISettingApplier settingApplier = new XIVDefaultSettingApplier();
            settingApplier.AddApplyCommand(new AntialiasingApplyCommand());
            settingApplier.AddApplyCommand(new ResolutionApplyCommand());
            settingApplier.AddApplyCommand(new ShadowQualityApplyCommand());
            settingApplier.AddApplyCommand(new TextureQualityApplyCommand());
            settingApplier.AddApplyCommand(new DisplayTypeApplyCommand());
            settingApplier.AddApplyCommand(new VsyncApplyCommand());
            return settingApplier;
        }

        SettingPreset[] CreateSettingPresets()
        {
            int length = presets.Length;
            var graphicPresets = new SettingPreset[length];

            for (int i = 0; i < length; i++)
            {
                graphicPresets[i] = presets[i].GetGraphicSetting();
            }

            return graphicPresets;
        }

        object ISavable.GetSaveData()
        {
            var preset = graphicSettingContainer.GetSetting<SettingPreset>();
            return preset;
        }

        void ISavable.LoadSaveData(object data)
        {
            var preset = (SettingPreset)data;
            graphicSettingContainer.InitializeSettings(preset);
            graphicSettingContainer.ApplyChanges();
            graphicSettingContainer.ClearUndoHistory();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (presets == null) return;
            defaultPresetIndex = defaultPresetIndex < 0 ? 0 : defaultPresetIndex > presets.Length - 1 ? presets.Length - 1 : defaultPresetIndex;
        }
#endif
    }
}