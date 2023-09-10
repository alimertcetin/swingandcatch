using System;
using TheGame.UISystems.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UISystems
{
    public class MessageDialogUI : GameUI
    {
        [SerializeField] TMP_Text txt_Message;
        [SerializeField] Button btn_Accept;
        [SerializeField] Button btn_Reject;

        Action acceptAction;
        Action rejectAction;

        protected override void Awake()
        {
            Clear();
            base.Awake();
        }

        public void Initialize(string text, Action onAccept, Action onReject)
        {
            txt_Message.text = text;
            acceptAction = onAccept;
            rejectAction = onReject;
        }

        void OnEnable()
        {
            btn_Accept.onClick.AddListener(OnAcceptClicked);
            btn_Reject.onClick.AddListener(OnRejectClicked);
        }

        void OnDisable()
        {
            btn_Accept.onClick.RemoveListener(OnAcceptClicked);
            btn_Reject.onClick.RemoveListener(OnRejectClicked);
        }

        protected override void OnUIDeactivated()
        {
            Clear();
        }

        void OnAcceptClicked()
        {
            acceptAction.Invoke();
            Clear();
            UISystem.Hide<MessageDialogUI>();
        }

        void OnRejectClicked()
        {
            rejectAction.Invoke();
            Clear();
            UISystem.Hide<MessageDialogUI>();
        }

        void Clear()
        {
            txt_Message.text = string.Empty;
            acceptAction = null;
            rejectAction = null;
        }
    }
}