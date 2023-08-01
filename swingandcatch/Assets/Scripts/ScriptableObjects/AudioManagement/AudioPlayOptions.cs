using UnityEngine;
using XIV.Core.Utils;

namespace TheGame.ScriptableObjects.AudioManagement
{
    public readonly struct AudioPlayOptions
    {
        /// <summary>
        /// Type of the audio
        /// </summary>
        public readonly AudioType audioType;
        /// <summary>
        /// Clip to play
        /// </summary>
        public readonly AudioClip clip;
        /// <summary>
        /// Defines if the clip will loop or not. Ignored for effects.
        /// </summary>
        public readonly bool isLooped;
        /// <summary>
        /// Defines how music transition will happen. Ignored for effects
        /// </summary>
        public readonly EasingFunction.Function easingFunc;

        static readonly EasingFunction.Function linearEasing = EasingFunction.Linear;
        
        /// <param name="audioType">Type of the audio</param>
        /// <param name="clip">Clip to play</param>
        /// <param name="isLooped">Will this <paramref name="clip"/> loop</param>
        /// <param name="easingFunc">If set this will be used for fade in and out</param>
        public AudioPlayOptions(AudioType audioType, AudioClip clip, bool isLooped, EasingFunction.Function easingFunc)
        {
            this.audioType = audioType;
            this.clip = clip;
            this.isLooped = isLooped;
            this.easingFunc = easingFunc ?? linearEasing;
        }

        /// <summary>
        /// Creates and returns new <see cref="AudioPlayOptions"/> for Effect
        /// </summary>
        /// <param name="clip">Clip to play</param>
        /// <returns>New <see cref="AudioPlayOptions"/> for Effect</returns>
        public static AudioPlayOptions EffectPlayOptions(AudioClip clip)
        {
            return new AudioPlayOptions(AudioType.Effect, clip, false, null);
        }

        /// <summary>
        /// Creates and returns new <see cref="AudioPlayOptions"/> for Music
        /// </summary>
        /// <param name="clip">Clip to play</param>
        /// <returns>New <see cref="AudioPlayOptions"/> for Music</returns>
        public static AudioPlayOptions MusicPlayOptions(AudioClip clip)
        {
            return new AudioPlayOptions(AudioType.Music, clip, true, EasingFunction.Linear);
        }
    }
}