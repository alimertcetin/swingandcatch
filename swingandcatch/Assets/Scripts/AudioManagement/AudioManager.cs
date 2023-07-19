﻿using System;
using System.Collections;
using System.Collections.Generic;
using TheGame.SaveSystems;
using TheGame.ScriptableObjects.Channels;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using XIV.Core.Utils;

namespace TheGame.AudioManagement
{
    public class AudioManager : MonoBehaviour, ISavable
    {
        [SerializeField] AudioMixerParameterCollectionChannelSO onAudioMixerParametersLoadedChannel;
        [SerializeField] VoidChannelSO onSceneLoaded;
        [SerializeField] AudioPlayOptionsChannelSO audioPlayOptionsChannel;
        [SerializeField] AudioMixer audioMixer;

        const int DEFAULT_MUSIC_SOURCE_COUNT = 2;
        const int DEFAULT_EFFECT_SOURCE_COUNT = 8;

        readonly List<AudioSource> musicSources = new List<AudioSource>(DEFAULT_MUSIC_SOURCE_COUNT);
        readonly List<AudioSource> effectSources = new List<AudioSource>(DEFAULT_EFFECT_SOURCE_COUNT);

        AudioMixerParameterCollection parameterCollection;

        void Awake()
        {
            parameterCollection = new AudioMixerParameterCollection(audioMixer);
            CreateAudioSources();
        }

        void Start() => onAudioMixerParametersLoadedChannel.RaiseEvent(parameterCollection);

        void OnEnable()
        {
            audioPlayOptionsChannel.Register(OnPlayAudio);
            onSceneLoaded.Register(OnSceneLoaded);
        }

        void OnDisable()
        {
            audioPlayOptionsChannel.Unregister(OnPlayAudio);
            onSceneLoaded.Unregister(OnSceneLoaded);
        }

        void OnSceneLoaded()
        {
            onAudioMixerParametersLoadedChannel.RaiseEvent(parameterCollection);
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

        [System.Serializable]
        struct SaveData
        {
            public string[] keys;
            public float[] values; // 0-1 values
        }

        object ISavable.GetSaveData()
        {
            List<AudioMixerParameter> parameterList = ListPool<AudioMixerParameter>.Get();

            foreach (AudioMixerParameter parameter in parameterCollection.GetParameters())
            {
                parameterList.Add(parameter);
            }

            int count = parameterList.Count;
            var keys = new string[count];
            var values = new float[count];

            for (int i = 0; i < count; i++)
            {
                keys[i] = parameterList[i].parameterName;
                values[i] = parameterList[i].value01;
            }
            
            ListPool<AudioMixerParameter>.Release(parameterList);
            
            return new SaveData
            {
                keys = keys,
                values = values,
            };
        }

        void ISavable.LoadSaveData(object data)
        {
            var saveData = (SaveData)data;
            var keys = saveData.keys;
            var values = saveData.values;
            var length = keys.Length;

            for (int i = 0; i < length; i++)
            {
                parameterCollection.UpdateParameter(keys[i], values[i]);
            }

        }
    }
}