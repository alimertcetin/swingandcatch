using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UISystems.Components
{
    public class SettingToggle : MonoBehaviour
    {
        [SerializeField] TMP_Text txt_Label;
        public Toggle toggle;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (txt_Label == false) txt_Label = GetComponentInChildren<TMP_Text>();
            if (toggle == false) toggle = GetComponentInChildren<Toggle>();
        }
#endif
    }
}