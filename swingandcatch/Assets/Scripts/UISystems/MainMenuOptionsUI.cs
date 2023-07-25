using TheGame.UISystems.Components;
using TheGame.UISystems.Core;
using TheGame.UISystems.TabSystem;
using UnityEngine;

namespace TheGame.UISystems
{
    public class MainMenuOptionsUI : TabUI
    {
        [SerializeField] CustomButton btn_back;

        protected override void OnEnable()
        {
            base.OnEnable();
            btn_back.onClick.AddListener(GoBackToMainPage);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            btn_back.onClick.RemoveAllListeners();
        }

        void GoBackToMainPage()
        {
            UISystem.Hide<MainMenuOptionsUI>();
            UISystem.Show<MainMenuUI>();
        }
    }
}