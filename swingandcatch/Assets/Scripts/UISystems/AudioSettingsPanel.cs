using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UISystems
{
    public class AudioSettingsPanel : MonoBehaviour
    {
        [SerializeField] TMP_Text txt_AudioSettingName;
        [SerializeField] Slider slider;

        Action<string, float> onSliderValueChanged;
        string auidoMixerParameter;

        public void Initialize(string auidoMixerParameter, Action<string, float> onSliderValueChanged)
        {
            this.auidoMixerParameter = auidoMixerParameter;
            this.txt_AudioSettingName.text = GetDisplayString(auidoMixerParameter);
            this.onSliderValueChanged = onSliderValueChanged;
            
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        void OnSliderValueChanged(float value)
        {
            onSliderValueChanged.Invoke(auidoMixerParameter, value);
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