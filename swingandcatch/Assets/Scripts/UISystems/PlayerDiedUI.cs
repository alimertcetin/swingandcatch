using TheGame.UISystems.Core;
using UnityEngine;

namespace TheGame.UISystems
{
    public class PlayerDiedUI : GameUI
    {
        public override void Show()
        {
            base.Show();
            Debug.Log("Player Died!");
        }
    }
}