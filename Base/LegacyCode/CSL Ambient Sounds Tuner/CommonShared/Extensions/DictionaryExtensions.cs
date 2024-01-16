﻿using System.Collections.Generic;

namespace AmbientSoundsTuner2.CommonShared.Extensions
{
    /// <summary>
    /// This static class contains extensions to <see cref="T:IDictionary`2"/> classes.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the value associated with the specified key. If the key does not exist, it will be set to the specified default value.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="defaultValue">The default value to use when the key has not been found in the dictionary.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the is found; otherwise, the specified default value. This parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the dictionary contains an element with the specified key; otherwise, <c>false</c>.</returns>
        public static bool TryGetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue, out TValue value)
        {
            if (!dictionary.TryGetValue(key, out value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }
    }
}
