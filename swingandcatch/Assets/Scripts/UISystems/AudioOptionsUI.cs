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
        [SerializeField] AudioMixerOptionsChannelSO audioMixerOptionsChannel;

        protected override void Awake()
        {
            base.Awake();
            CreateUIItems();
        }

        void CreateUIItems()
        {
            var paramaters = AudioMixerConstants.DefaultMixer.Parameters.All;
            int length = paramaters.Length;
            for (int i = 0; i < length; i++)
            {
                var panel = Instantiate(auidoSettingPanelPrefab, contentParent, false);
                var mixerParameter = paramaters[i];
                panel.Initialize(mixerParameter, OnSliderValueChanged);
            }
        }

        void OnSliderValueChanged(string mixerParameter, float value)
        {
            value = Mathf.Log(Mathf.Max(value, 0.01f)) * 20f;
            audioMixerOptionsChannel.RaiseEvent(new AudioMixerOptions(mixerParameter, value));
        }
    }
}