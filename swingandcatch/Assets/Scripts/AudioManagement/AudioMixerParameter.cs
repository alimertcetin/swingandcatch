using UnityEngine;

namespace TheGame.AudioManagement
{
    public struct AudioMixerParameter
    {
        public readonly string parameterName;
        public readonly int parameterNameHash;
        public float logarithmicValue { get; private set; }
        public float value01 { get; private set; }

        const float MIN_VALUE = 0.01F;

        public AudioMixerParameter(string name, float value01)
        {
            this.parameterName = name;
            this.parameterNameHash = name.GetHashCode();
            this.value01 = Mathf.Max(value01, MIN_VALUE);;
            this.logarithmicValue = GetLogarithmicValue(this.value01);
        }

        public void UpdateValue01(float newValue01)
        {
            value01 = Mathf.Max(newValue01, MIN_VALUE);
            logarithmicValue = GetLogarithmicValue(value01);
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