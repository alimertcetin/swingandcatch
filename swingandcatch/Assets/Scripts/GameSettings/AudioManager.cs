using System;
using System.Collections;
using System.Collections.Generic;
using TheGame.ScriptableObjects.AudioManagement;
using TheGame.ScriptableObjects.Channels;
using TheGame.SettingSystems;
using UnityEngine;
using UnityEngine.Audio;
using XIV.Core.Utils;
using AudioType = TheGame.ScriptableObjects.AudioManagement.AudioType;

namespace TheGame.GameSettings
{
    // TODO : AudioManager -> SOLID
    public class AudioManager : MonoBehaviour, ISettingsListener
    {
        [SerializeField] SettingsChannelSO settingsLoaded;
        [SerializeField] AudioPlayOptionsChannelSO audioPlayOptionsChannel;
        [SerializeField] AudioMixer audioMixer;

        const int DEFAULT_MUSIC_SOURCE_COUNT = 2;
        const int DEFAULT_EFFECT_SOURCE_COUNT = 8;

        readonly List<AudioSource> musicSources = new List<AudioSource>(DEFAULT_MUSIC_SOURCE_COUNT);
        readonly List<AudioSource> effectSources = new List<AudioSource>(DEFAULT_EFFECT_SOURCE_COUNT);

        Settings settings;

        void Awake()
        {
            CreateAudioSources();
        }

        void OnEnable()
        {
            audioPlayOptionsChannel.Register(OnPlayAudio);
            settingsLoaded.Register(OnSettingsLoaded);
            this.settings?.AddListener(this);
        }

        void OnDisable()
        {
            audioPlayOptionsChannel.Unregister(OnPlayAudio);
            settingsLoaded.Unregister(OnSettingsLoaded);
            this.settings?.RemoveListener(this);
        }

        void OnSettingsLoaded(Settings settings)
        {
            this.settings = settings;
            this.settings.AddListener(this);
            SetMixerParameters();
        }

        void SetMixerParameters()
        {
            var masterVolume = settings.GetParameter(SettingParameterType.Audio, AudioSettingsParameterContainer.masterVolumeHash).ReadValue<float>();
            masterVolume = masterVolume < 0.01f ? 0.01f : masterVolume;
            var musicVolume = settings.GetParameter(SettingParameterType.Audio, AudioSettingsParameterContainer.musicVolumeHash).ReadValue<float>();
            musicVolume = musicVolume < 0.01f ? 0.01f : musicVolume;
            var effectVolume = settings.GetParameter(SettingParameterType.Audio, AudioSettingsParameterContainer.effectsVolumeHash).ReadValue<float>();
            effectVolume = effectVolume < 0.01f ? 0.01f : effectVolume;
            
            audioMixer.SetFloat(AudioMixerConstants.DefaultMixer.Parameters.MasterVolume, GetLogarithmicValue(masterVolume));
            audioMixer.SetFloat(AudioMixerConstants.DefaultMixer.Parameters.MusicVolume, GetLogarithmicValue(musicVolume));
            audioMixer.SetFloat(AudioMixerConstants.DefaultMixer.Parameters.EffectsVolume, GetLogarithmicValue(effectVolume));
        }

        void CreateAudioSources()
        {
            var musicGroup = audioMixer.FindMatchingGroups(AudioMixerConstants.DefaultMixer.MixerGroups.Musics)[0];
            var effectsGroup = audioMixer.FindMatchingGroups(AudioMixerConstants.DefaultMixer.MixerGroups.Effects)[0];

            var musicSourceParent = new GameObject("--- Music Sources ---").transform;
            var effectSourceParent = new GameObject("--- Effect Sources ---").transform;
            musicSourceParent.SetParent(this.transform);
            effectSourceParent.SetParent(this.transform);

            for (int i = 0; i < DEFAULT_MUSIC_SOURCE_COUNT; i++)
            {
                var source = new GameObject("Music-Source_" + i).AddComponent<AudioSource>();
                InitializeSource(source, musicSourceParent, musicGroup);
                musicSources.Add(source);
            }

            for (int i = 0; i < DEFAULT_EFFECT_SOURCE_COUNT; i++)
            {
                var source = new GameObject("Effect-Source_" + i).AddComponent<AudioSource>();
                InitializeSource(source, effectSourceParent, effectsGroup);
                effectSources.Add(source);
            }
        }

        static void InitializeSource(AudioSource source, Transform parent, AudioMixerGroup group)
        {
            source.transform.SetParent(parent);
            source.outputAudioMixerGroup = group;
            source.spatialBlend = 0f;
            source.volume = 1f;
        }

        void OnPlayAudio(AudioPlayOptions audioPlayOptions)
        {
            switch (audioPlayOptions.audioType)
            {
                case AudioType.None: return;
                case AudioType.Music:
                    PlayMusic(audioPlayOptions);
                    break;
                case AudioType.Effect:
                    PlayEffect(audioPlayOptions);
                    break;
                default: throw new NotImplementedException(audioPlayOptions.audioType + " is not implemented.");
            }
        }

        void PlayMusic(AudioPlayOptions audioPlayOptions)
        {
            AudioSource targetSource;
            AudioSource currentSource;
            if (musicSources[0].isPlaying == false)
            {
                targetSource = musicSources[0];
                currentSource = musicSources[1];
            }
            else
            {
                targetSource = musicSources[1];
                currentSource = musicSources[0];
            }

            targetSource.Stop();
            targetSource.clip = audioPlayOptions.clip;
            targetSource.loop = audioPlayOptions.isLooped;
            targetSource.Play();
            StartCoroutine(FadeInOut(targetSource, currentSource, 1f, audioPlayOptions.easingFunc));
        }

        void PlayEffect(AudioPlayOptions audioPlayOptions)
        {
            if (TryGetAvailableSource(effectSources, out var source))
            {
                source.PlayOneShot(audioPlayOptions.clip);
            }
            else
            {
                CreateNewSource(effectSources).PlayOneShot(audioPlayOptions.clip);
            }
        }

        IEnumerator FadeInOut(AudioSource sourceToFadeIn, AudioSource sourceToFadeOut, float duration, EasingFunction.Function easingFunc)
        {
            var timer = new Timer(duration);

            while (timer.IsDone == false)
            {
                timer.Update(Time.deltaTime);

                var t = timer.NormalizedTime;
                var fadeInT = easingFunc.Invoke(0f, 1f, t);
                var fadeOutT = easingFunc.Invoke(0f, 1f, 1f - t);
                
                sourceToFadeIn.volume = fadeInT;
                sourceToFadeOut.volume = fadeOutT;
                
                yield return null;
            }
            
            sourceToFadeOut.Stop();
        }

        bool TryGetAvailableSource(IList<AudioSource> audioSources, out AudioSource source)
        {
            source = default;
            var count = audioSources.Count;
            for (int i = 0; i < count; i++)
            {
                source = audioSources[i];
                if (source.isPlaying == false) return true;
            }

            return false;
        }

        static AudioSource CreateNewSource(IList<AudioSource> sources)
        {
            var source = sources[^1];
            var name = source.name.Substring(0, source.name.Length - 1);
            name += "_" + sources.Count;
            var newSource = new GameObject(name).AddComponent<AudioSource>();
            InitializeSource(newSource, source.transform.parent, source.outputAudioMixerGroup);
            sources.Add(newSource);
            return newSource;
        }

        void ISettingsListener.OnSettingsChanged(SettingParameter changedParameter)
        {
            if (changedParameter.settingParameterType != SettingParameterType.Audio) return;

            var value01 = changedParameter.ReadValue<float>();
            value01 = value01 < 0.01f ? 0.01f : value01;
            var logarithmicValue = GetLogarithmicValue(value01);
#if UNITY_EDITOR
            if (audioMixer.SetFloat(changedParameter.name, logarithmicValue) == false)
            {
                Debug.LogError(changedParameter.name + " is not found in audio mixer");
            }
#else
            audioMixer.SetFloat(changedParameter.name, logarithmicValue);
#endif
        }

        /// <summary>
        /// This may return <see cref="float.NegativeInfinity"/> if value equals to 0
        /// </summary>
        /// <param name="value01">Value between 0 and 1</param>
        /// <returns>A logarithmic value to use in <see cref="UnityEngine.Audio.AudioMixer"/></returns>
        public static float GetLogarithmicValue(float value01)
        {
            return Mathf.Log(value01) * 20f;
        }

        /// <summary>
        /// Returns a value between 0 and 1
        /// </summary>
        /// <param name="logarithmicValue">Value that used in <see cref="UnityEngine.Audio.AudioMixer"/></param>
        /// <returns>A value between 0 and 1</returns>
        public static float GetValue01(float logarithmicValue)
        {
            return Mathf.Exp(logarithmicValue / 20f);
        }
    }
}