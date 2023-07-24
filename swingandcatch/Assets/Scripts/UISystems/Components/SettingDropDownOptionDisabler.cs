using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UISystems.Components
{
    [RequireComponent(typeof(Toggle))]
    public class SettingDropDownOptionDisabler : MonoBehaviour
    {
        public bool ignoreCase = true;
        public string[] optionsToDisable;
        Toggle toggle;

        void Start()
        {
            toggle = GetComponent<Toggle>();

            int optionToDisableLength = optionsToDisable.Length;
            for (int i = 0; i < optionToDisableLength; i++)
            {
                optionsToDisable[i] = ignoreCase ? optionsToDisable[i].Replace(" ", "").ToLower(CultureInfo.InvariantCulture) : optionsToDisable[i].Replace(" ", "");
            }
            
            var nameSplitted = ignoreCase ? toggle.name.Replace(" ", "").ToLower(CultureInfo.InvariantCulture).Split(':') : toggle.name.Replace(" ", "").Split(':');
            int nameSplittedLength = nameSplitted.Length;
            for (int i = 0; i < nameSplittedLength; i++)
            {
                for (int j = 0; j < optionToDisableLength; j++)
                {
                    if (optionsToDisable[j] != nameSplitted[i]) continue;
                    toggle.interactable = false;
                    break;
                }
            }
            
        }
    }
}