using TheGame.UISystems.Core;
using UnityEngine;

namespace TheGame.UISystems
{
    public class HudUI : ParentGameUI
    {
        [SerializeField] HudCoinPageUI coinPageUI;
        [SerializeField] HudHealthPageUI healthPageUI;
        public Vector3 coinUIItemRectPosition => coinPageUI.coinUIItemRectPosition;
        
    }
}