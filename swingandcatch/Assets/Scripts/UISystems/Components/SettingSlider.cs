using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UISystems.Components
{
    public class SettingSlider : MonoBehaviour
    {
        public TMP_Text txt_Label;
        public Slider slider;
        public TMP_Text txt_SliderPercent;

        public void UpdateValue(float newValue, bool updateWithoutNotify)
        {
            if (updateWithoutNotify) slider.SetValueWithoutNotify(newValue);
            else slider.value = newValue;

            txt_SliderPercent.text = "% " + ((int)(newValue * 100f)).ToString();
        }
    }
}