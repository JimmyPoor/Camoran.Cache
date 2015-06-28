using Camoran.Cache.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camoran.Cache.Memory
{
    public class CamoranMemoryCache<Key, Value> : CamoranCache<Key, Value>
    {
        protected IMemoryCacheStrategy<Key, CacheObject> MemoryCacheStrategy { get; private set; }
        public CamoranMemoryCache(IMemoryCacheStrategy<Key, CacheObject> memoryCacheStrategy)
        {
            this.MemoryCacheStrategy = memoryCacheStrategy;
        }

        public override Value Get(Key key)
        {
            CacheObject cahceObj = MemoryCacheStrategy.Get(key);
            return cahceObj.IsExpire() ? default(Value) : cahceObj.Value;
        }

        public override void Set(Key key, Value value, TimeSpan expireTime)
        {
            var cahceObj = new CacheObject(key, value);
            cahceObj.SetExpire(expireTime);
            var expireDate=DateTime.Now.Add(expireTime);
            MemoryCacheStrategy.SetExpire(key, expireDate);
            MemoryCacheStrategy.Set(key, cahceObj);
        }

        public override void Remove(Key key)
        {
            MemoryCacheStrategy.Remove(key);
        }
    }
}
