using Camoran.Cache.Core;
using ServiceStack.Redis;
using System;

namespace Camoran.Cache.Redis
{
    public class CamoranRedisCache<Value> : CamoranCache<string, Value>, IDisposable
    {
         static IRedisClient _redisClient;
         static PooledRedisClientManager _pooledRedisManger;
         protected IRedisCacheStrategy<string, CacheObject> CacheStrategy { get; set; }
        public CamoranRedisCache() 
        {
            _pooledRedisManger = CreateClientManager(null, null);
            _redisClient = _pooledRedisManger.GetClient();
        }

        static PooledRedisClientManager CreateClientManager(string[] readHosts, string[] writerHosts) 
        {
            if (readHosts == null) throw new ArgumentNullException("read host array can't be null");
            return new PooledRedisClientManager(readHosts,writerHosts);
        }

        public override Value Get(string key)
        {
             CacheObject obj = CacheStrategy.Get(key);
             DateTime strategyExpireDate=DateTime.Now.Add(obj.ExpireTime);
             return obj.IsExpire() ? default(Value) : obj.Value;
        }

        public override void Set(string key, Value value, TimeSpan expireTime)
        {
            CacheObject obj = new CacheObject(key, value);
            obj.SetExpire(expireTime);
            CacheStrategy.SetExpire(key, DateTime.Now.Add(obj.ExpireTime));
            CacheStrategy.Set(obj.Key, obj);
        }

        public override void Remove(string key)

        {
            CacheStrategy.Remove(key);
        }


        public void Dispose()
        {
            _pooledRedisManger.Dispose();
            _redisClient.Dispose();
        }
    }
}
