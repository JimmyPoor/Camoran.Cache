using Camoran.Cache.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camoran.Cache.Memory.CacheStrategy
{
    public class MemoryConcurrencyCacheStrategy<Key, Value> : IMemoryCacheStrategy<Key, Value>
    {
        ConcurrentDictionary<Key, CacheVal> caches = new ConcurrentDictionary<Key, CacheVal>();
        private TimeSpan _expireSpan;
        public MemoryConcurrencyCacheStrategy(TimeSpan expire)
        {
            this._expireSpan = expire;
        }


        public void Set(Key key, Value value)
        {
            CacheVal cv = new CacheVal(key, value, DateTime.Now.Add(_expireSpan));
            caches.AddOrUpdate(key
               , (k) =>
               {
                   return cv;
               }
               , (k, old) =>
               {
                   return old.Equals(value) ? old : cv;
               });
        }

        public bool Remove(Key key)
        {
            CacheVal cacheVal = null;
            if (caches.ContainsKey(key))
            {
                return caches.TryRemove(key, out cacheVal);
            }
            return false;
        }

        public void SetExpire(Key key, DateTime expireDate)
        {
            foreach (Key keyObj in this.caches.Keys) 
            {
                if (keyObj.Equals(key)) 
                {
                    caches[keyObj].ExpireDate = expireDate;
                }
            }
        }


        public Value Get(Key key)
        {
            CacheVal cacheVal = null;
            if (caches.ContainsKey(key))
            {
                this.caches.TryGetValue(key, out cacheVal);
            }
            return cacheVal.IsExpire ? default(Value) : cacheVal.Val;
        }

        private class CacheVal
        {
            public CacheVal(Key key,Value val, DateTime expireDate)
            {
                this.Val = val;
                this.Key = key;
                this.ExpireDate = expireDate;
            }

            public Key Key { get; private set; }

            public Value Val { get; private set; }

            public DateTime ExpireDate { get;  set; }

            public bool IsExpire
            {
                get
                {
                    return DateTime.Now >= ExpireDate;
                }
            }
        }

    }
}
