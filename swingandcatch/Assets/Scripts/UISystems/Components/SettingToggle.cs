using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UISystems.Components
{
    public class SettingToggle : MonoBehaviour
    {
        public TMP_Text txt_Label;
        public Toggle toggle;

        public void UpdateValue(bool newValue, bool updateWithoutNotify)
        {
            if (updateWithoutNotify) toggle.SetIsOnWithoutNotify(newValue);
            else toggle.isOn = newValue;
        }
    }
}