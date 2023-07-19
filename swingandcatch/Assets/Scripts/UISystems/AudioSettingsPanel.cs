using System.Text.RegularExpressions;
using TheGame.AudioManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UISystems
{
    public class AudioSettingsPanel : MonoBehaviour
    {
        [SerializeField] TMP_Text txt_AudioSettingName;
        [SerializeField] Slider slider;
        
        AudioMixerParameterCollection parameterCollection;
        AudioMixerParameter parameter;

        public void Initialize(AudioMixerParameterCollection parameterCollection, AudioMixerParameter audioMixerParameter)
        {
            this.parameter = audioMixerParameter;
            this.txt_AudioSettingName.text = GetDisplayString(audioMixerParameter.parameterName);
            this.parameterCollection = parameterCollection;
            slider.SetValueWithoutNotify(parameter.value01);
        }

        void OnEnable() => slider.onValueChanged.AddListener(OnSliderValueChanged);

        void OnDisable() => slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        
        void OnSliderValueChanged(float value)
        {
            parameter.UpdateValue01(value);
            parameterCollection.UpdateParameter(parameter.parameterNameHash, parameter.value01);
        }
        
        static string GetDisplayString(string audioSettingName)
        {
            var splitArr = Regex.Split(audioSettingName, @"(?<!^)(?=[A-Z])");
            int length = splitArr.Length;
            string fullString = "";
            for (int i = 0; i < length; i++)
            {
                fullString += splitArr[i] + " ";
            }

            fullString = fullString.TrimEnd(' ');

            return fullString;
        }
    }
}