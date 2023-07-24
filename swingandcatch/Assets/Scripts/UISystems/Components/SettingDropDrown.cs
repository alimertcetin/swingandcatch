using TMPro;
using UnityEngine;

namespace TheGame.UISystems.Components
{
    public class SettingDropDrown : MonoBehaviour
    {
        public TMP_Text txt_Label;
        public TMP_Dropdown dropDown;

        public void UpdateValue(int newValue, bool updateWithoutNotify)
        {
            if (updateWithoutNotify) dropDown.SetValueWithoutNotify(newValue);
            else dropDown.value = newValue;
        }
    }
}