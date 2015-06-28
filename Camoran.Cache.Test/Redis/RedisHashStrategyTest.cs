using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camoran.Cache.Test.TestModel;
using Camoran.Cache.Redis.CacheStrategy;
using ServiceStack.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Camoran.Cache.Test.Redis
{
    [TestClass]
    public class RedisHashStrategyTest
    {
 
        private PooledRedisClientManager _pooledRedisManger;
        private IRedisClient _client;
        private string[] serverHost = { "127.0.0.1:6379" };
        public RedisHashStrategyTest()
        {
            _pooledRedisManger = new PooledRedisClientManager(serverHost);
            _client = _pooledRedisManger.GetClient();
        }

        [TestMethod]
        public void SetCache_SingleModel_Test()
        {
            RedisCache_HashSrategy<CacheModel> _hashStrategy = new RedisCache_HashSrategy<CacheModel>(_client);
            CacheModel model = new CacheModel { Age = 19 };
            _hashStrategy.Set("key", new CacheModel { Age = 19, NickName = "kissnana" });
            var model2 = _hashStrategy.Get("key");
            Assert.AreEqual(model.Age, model2.Age);
            Assert.AreEqual(model.NickName, model2.NickName);
            Assert.AreNotEqual(model, model2);
        }

        [TestMethod]
        public void SetCache_ListModel_Test()
        {
            RedisCache_HashSrategy<IList<CacheModel>> _hashStrategy = new RedisCache_HashSrategy<IList<CacheModel>>(_client);
            IList<CacheModel> models = new List<CacheModel> 
            {
                new CacheModel { Age = 11, NickName = "kissnana2" },
                new CacheModel { Age = 10, NickName = "kissnana" }
            };
            _hashStrategy.Set("listKey", models);
            var models2=_hashStrategy.Get("listKey");
            Assert.AreEqual(models.Count,models2.Count);
            Assert.IsTrue(models2.Any(x=>x.NickName=="kissnana" && x.Age==10));
        }



        [TestMethod]

        public void Cache_Expire_Test() 
        {
            string key="expireKey";
            DateTime expireDate = DateTime.Now.Add(new TimeSpan(0,0,1));
            RedisCache_HashSrategy<CacheModel> _hashStrategy = new RedisCache_HashSrategy<CacheModel>(_client);
            var model = new CacheModel { Age = 19 };
            _hashStrategy.Set(key, model);
            _hashStrategy.SetExpire(key,expireDate);
          
            Thread.Sleep(5000);
            var model2=_hashStrategy.Get(key);

            Assert.IsTrue(model2==null);
        }



    }




}
