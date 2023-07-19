namespace TheGame.AudioManagement
{
    public interface IAudioMixerParameterListener
    {
        void OnParameterChanged(AudioMixerParameter parameter);
    }
}