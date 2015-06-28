using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


namespace Camoran.Cache.Redis.CacheStrategy
{
    public class RedisCache_HashSrategy<Value>:IRedisCacheStrategy<string,Value> 
    {
        private IRedisClient _client;
        private JavaScriptSerializer _serializer;

        public RedisCache_HashSrategy(IRedisClient client) 
        {
            this._client = client;
            _serializer = new JavaScriptSerializer();
        }

        public Func<string, string> BuildHash { get; set; }

        public void Set(string key, Value value)
        {
            if (key == null) { }
            string hash = CreateKeyHashFromString(key);
            string jsonValue = _serializer.Serialize(value);
            _client.SetEntryInHashIfNotExists(hash, key, jsonValue);
        }

        public Value Get(string key)
        {
           string hash = CreateKeyHashFromString(key);
           string value= _client.GetValueFromHash(hash, key);
           return _serializer.Deserialize<Value>(value);
        }


        public bool Remove(string key)
        {
            string hash = CreateKeyHashFromString(key);
            return _client.RemoveEntryFromHash(hash, key);
        }

        public void SetExpire(string key, DateTime expireDate)
        {
             _client.ExpireEntryAt(key, expireDate);
        }


        private string CreateKeyHashFromString(string key) 
        {
            string hash = BuildHash == null ?
              key.GetHashCode().ToString()
              : BuildHash(key);
            return hash;
        }

    }
}
