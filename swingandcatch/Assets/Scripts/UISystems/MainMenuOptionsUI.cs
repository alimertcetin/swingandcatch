using TheGame.UISystems.Components;
using TheGame.UISystems.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TheGame.UISystems
{
    public class MainMenuOptionsUI : ParentGameUI
    {
        [SerializeField] AudioOptionsUI audioOptionsUI;
        [SerializeField] CustomButton btn_back;

        void OnEnable()
        {
            btn_back.onClick.AddListener(GoBackToMainPage);
        }

        void OnDisable()
        {
            btn_back.onClick.RemoveAllListeners();
        }

        public override void Show()
        {
            EventSystem.current.SetSelectedGameObject(btn_back.gameObject);
            audioOptionsUI.Show();
            base.Show();
        }

        public override void Hide()
        {
            audioOptionsUI.Hide();
            base.Hide();
        }

        void GoBackToMainPage()
        {
            UISystem.Hide<MainMenuOptionsUI>();
            UISystem.Show<MainMenuUI>();
        }
    }
}