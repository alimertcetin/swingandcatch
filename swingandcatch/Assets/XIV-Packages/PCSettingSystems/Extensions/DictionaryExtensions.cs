using System.Collections.Generic;

// TODO : Move to XIV.Core.Extensions?
namespace XIV_Packages.PCSettingSystems.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// If key doesn't exists adds the specified key and value to the dictionary. Otherwise replaces the existing value.
        /// </summary>
        /// <returns><see langword="true"/> if new value added. <see langword="false"/> if it is replaced</returns>
        public static bool AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            var hasKey = dictionary.ContainsKey(key);
            if (hasKey) dictionary[key] = value;
            else dictionary.Add(key, value);
            return hasKey == false;
        }

        /// <summary>
        /// Removes all keys if found in <paramref name="dictionary"/>
        /// </summary>
        public static void RemoveKeys<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IList<TKey> keys)
        {
            int count = keys.Count;
            for (int i = 0; i < count; i++)
            {
                dictionary.Remove(keys[i]);
            }
        }
    }
}