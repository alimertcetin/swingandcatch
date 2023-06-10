using System;
using UnityEngine;

namespace TheGame.SettingSystems
{
    public class GameSettings
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeSettings()
        {
            SetFrameRate(60);
        }
        
        static void SetFrameRate(int frameRate)
        {
            Application.targetFrameRate = frameRate;
        }
    }
}