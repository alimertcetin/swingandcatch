namespace TheGame.AudioManagement
{
    public readonly struct AudioMixerOptions
    {
        public readonly string parameter;
        public readonly float value;

        public AudioMixerOptions(string parameter, float value)
        {
            this.parameter = parameter;
            this.value = value;
        }
    }
}