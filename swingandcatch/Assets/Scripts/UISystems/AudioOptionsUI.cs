using System.Collections.Generic;
using TheGame.AudioManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.UISystems.Core;
using UnityEngine;

namespace TheGame.UISystems
{
    public class AudioOptionsUI : GameUI
    {
        [SerializeField] RectTransform contentParent;
        [SerializeField] AudioSettingsPanel auidoSettingPanelPrefab;
        [SerializeField] AudioMixerParameterCollectionChannelSO onAudioMixerParametersLoaded;

        AudioMixerParameterCollection parameterCollection;
        readonly List<AudioSettingsPanel> audioSettingsPanels = new();

        void OnEnable()
        {
            onAudioMixerParametersLoaded.Register(OnParameterCollectionLoaded);
        }

        void OnDisable()
        {
            onAudioMixerParametersLoaded.Unregister(OnParameterCollectionLoaded);
        }

        void OnParameterCollectionLoaded(AudioMixerParameterCollection parameterCollection)
        {
            this.parameterCollection = parameterCollection;
            InitializeUIItems();
        }

        void InitializeUIItems()
        {
            foreach (AudioMixerParameter parameter in parameterCollection.GetParameters())
            {
                var panel = Instantiate(auidoSettingPanelPrefab, contentParent, false);
                panel.Initialize(parameterCollection, parameter);
                audioSettingsPanels.Add(panel);
            }
        }
    }
}