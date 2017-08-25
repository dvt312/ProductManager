using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Ajax.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace ProductManager.Business.Cache
{
    /// <summary>
    /// This class will handle a temporal cache for quick access to objects through requests.
    /// </summary>
    public class CacheManager
    {
        /// <summary>
        /// The time on which the cached object will expired and removed from the cache.
        /// </summary>
        private const int EXPIRING_TIME = 15;

        /// <summary>
        /// The lock object used to prevent removing an adding object.
        /// </summary>
        private static readonly object _lockThis = new object();

        /// <summary>
        /// The cache itself.
        /// </summary>
        private static readonly Dictionary<string, CacheItem> _cacheDictionary =
            new Dictionary<string, CacheItem>();

        /// <summary>
        /// A queue to be deleted.
        /// </summary>
        private static readonly List<string> _queueToDelete = new List<string>();


        /// <summary>
        /// A timer which will run each minute removing the oldest object if its time has expired.
        /// </summary>
        private static Timer _garbageTimer = CreateCleanCacheTimer();


        /// <summary>
        /// Stores an object in the cache.
        /// </summary>
        /// <param name="obj">The object to be stored.</param>
        /// <param name="guid">An unique id representing the object.</param>
        /// <returns>The unique id that represents the object.</returns>
        public static string CacheObject(object obj, string guid = null)
        {
            string id = guid ?? Guid.NewGuid().ToString();
            CacheItem newItem = new CacheItem()
            {
                LastUpdateTime = DateTime.Now,
                StoreDateTime = DateTime.Now,
                StoreItem = obj
            };

            lock (_lockThis)
            {
                if (ExistCachedObject(id))
                {
                    _cacheDictionary.Remove(id);
                    _queueToDelete.Remove(id);
                }

                _cacheDictionary.Add(id, newItem);
                _queueToDelete.Add(id);
            }

            return id;
        }

        /// <summary>
        /// Gets the object that is represented by the given id.
        /// </summary>
        /// <param name="id">The id to be looked for.</param>
        /// <returns>The finded object. null if the id does not exist.</returns>
        public static object RetrieveObjectById(string id)
        {
            object obj = null;

            if (!id.IsNullOrWhiteSpace())
            {
                _cacheDictionary[id].LastUpdateTime = DateTime.Now;
                obj = _cacheDictionary[id].StoreItem;
            }

            return obj;
        }

        /// <summary>
        /// Checks if the given id represents an object.
        /// </summary>
        /// <param name="id">The if to be looked for.</param>
        /// <returns>True if the id exists, false otherwise.</returns>
        public static bool ExistCachedObject(string id)
        {
            return id != null && _cacheDictionary.ContainsKey(id);
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public static void CleanCache()
        {
            _cacheDictionary.Clear();
        }


        [ExcludeFromCodeCoverage]
        private static void OnCleanCacheEvent(object source, ElapsedEventArgs e)
        {
            if (_queueToDelete.Count <= 0)
            {
                return;
            }
            lock (_lockThis)
            {
                if (_cacheDictionary[_queueToDelete[0]].StoreDateTime.AddMinutes(EXPIRING_TIME)
                    >= DateTime.Now)
                {
                    return;
                }
                _cacheDictionary.Remove(_queueToDelete[0]);
                _queueToDelete.RemoveAt(0);
            }
        }

        private static Timer CreateCleanCacheTimer()
        {
            Timer timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(OnCleanCacheEvent);
            timer.Interval = 60000;
            timer.Start();

            return timer;
        }

    }
}