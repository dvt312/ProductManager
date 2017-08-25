using System;

namespace ProductManager.Business.Cache
{
    /// <summary>
    /// This class represents a cached item.
    /// </summary>
    internal class CacheItem
    {
        /// <summary>
        /// The item stored in the cache.
        /// </summary>
        internal object StoreItem { get; set; }

        /// <summary>
        /// The time on which the object was stored.
        /// </summary>
        internal DateTime StoreDateTime { get; set; }

        /// <summary>
        /// The last time the object was accesed.
        /// </summary>
        internal DateTime LastUpdateTime { get; set; }
    }
}