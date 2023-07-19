using System.Collections.Generic;
using UnityEngine.Audio;
using XIV.Core.Collections;

namespace TheGame.AudioManagement
{
    public class AudioMixerParameterCollection
    {
        AudioMixer mixer;
        DynamicArray<AudioMixerParameter> parameters = new DynamicArray<AudioMixerParameter>(2);
        List<IAudioMixerParameterListener> listeners = new List<IAudioMixerParameterListener>();
        
        public AudioMixerParameterCollection(AudioMixer mixer)
        {
            this.mixer = mixer;
            var parameters = AudioMixerConstants.DefaultMixer.Parameters.All;
            int length = parameters.Length;

            for (int i = 0; i < length; i++)
            {
                var parameterName = parameters[i];
                if (mixer.GetFloat(parameterName, out var logarithmicValue))
                {
                    var option = new AudioMixerParameter(parameterName, AudioMixerParameter.GetValue01(logarithmicValue));
                    this.parameters.Add() = option;
                }
            }
        }

        public void AddListener(IAudioMixerParameterListener listener)
        {
            if (listeners.Contains(listener)) return;
            listeners.Add(listener);
        }

        public bool RemoveListener(IAudioMixerParameterListener listener) => listeners.Remove(listener);

        public bool UpdateParameter(string parameterName, float value01)
        {
            var parameterNameHash = parameterName.GetHashCode();
            return UpdateParameter(parameterNameHash, value01);
        }

        public bool UpdateParameter(int parameterNameHash, float value01)
        {
            int index = parameters.Exists((p) => p.parameterNameHash == parameterNameHash);
            if (index < 0) return false;
            ref var parameter = ref parameters[index];
            parameter.UpdateValue01(value01);
            mixer.SetFloat(parameter.parameterName, parameter.logarithmicValue);
            InformListeners(parameter);
            return true;
        }

        public IEnumerable<AudioMixerParameter> GetParameters()
        {
            int count = parameters.Count;
            for (int i = 0; i < count; i++)
            {
                yield return parameters[i];
            }
        }

        void InformListeners(AudioMixerParameter parameter)
        {
            int count = listeners.Count;
            for (int i = 0; i < count; i++)
            {
                listeners[i].OnParameterChanged(parameter);
            }
        }
    }
}