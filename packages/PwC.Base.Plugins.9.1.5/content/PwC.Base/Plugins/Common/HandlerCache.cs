using System.Collections.Generic;

namespace PwC.Base.Plugins.Common
{
    /// <summary>
    /// Handler Cache is a local plugin cache that is passed in plugin handlers.
    /// It helps to store and distribute some values among different plugin handlers.
    /// </summary>
    public class HandlerCache
    {
        public HandlerCache()
        {
            this.LocalDictinary = new Dictionary<string, object>();
        }

        private Dictionary<string, object> LocalDictinary { get; set; }

        /// <summary>
        /// Adds a value under a given key to a cache dictionary.
        /// </summary>
        public object this[string key]
        {
            set
            {
                this.LocalDictinary[key] = value;
            }
        }

        /// <summary>
        /// Gets cached value from a given cache dictionary key. Returns default if not found.
        /// </summary>
        /// <typeparam name="T">Type of a cached value to retrieve.</typeparam>
        /// <param name="key">Cache dictionary value key.</param>
        /// <returns>Value of a given cache parameter name and projects it on a given type.</returns>
        public T GetFromCache<T>(string key)
        {
            object obj;
            if (this.LocalDictinary.TryGetValue(key, out obj))
            {
                if (obj is T)
                {
                    return (T)obj;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Determines whether cache dictionary [contains] [the specified key].
        /// </summary>
        /// <param name="key">Cached dictionary value key.</param>
        /// <returns>
        ///   <c>true</c> if cache dictionary [contains] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string key)
        {
            return this.LocalDictinary.ContainsKey(key);
        }
    }
}