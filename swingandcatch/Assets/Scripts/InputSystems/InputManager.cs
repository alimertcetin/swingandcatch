﻿namespace TheGame.Scripts.InputSystems
{
    public static class InputManager
    {
        static DefaultGameInputs inputs;
        public static DefaultGameInputs Inputs => inputs ??= new DefaultGameInputs();

        public static void DisableAll()
        {
            var inputs = Inputs;
            inputs.PlayerClimb.Disable();
            inputs.PlayerGrounded.Disable();
        }
    }
}