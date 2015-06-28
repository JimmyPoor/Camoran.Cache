using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camoran.Cache.Core
{
    public abstract class CamoranCache<Key,Value> :ICahce<Key,Value>
    {
        public abstract Value Get(Key key);

        public abstract void Set(Key key, Value value, TimeSpan expireTime);

        public abstract void Remove(Key key);


        public class CacheObject
        {
            public Key Key { get; private set; }
            public Value Value { get; private set; }
            public DateTime CreateDate { get; set; }
            public TimeSpan ExpireTime { get; private set; }
            public CacheObject(Key key, Value value)
            {
                this.Key = key;
                this.Value = value;
            }

            public void SetExpire(TimeSpan expireTime) 
            {
                this.ExpireTime = expireTime;
            }

            public bool IsExpire() 
            {
                return DateTime.Now - CreateDate >= ExpireTime;
            }
        }



      
    }
}
