using EasyDo.Cache;
using EasyDo.Dependency;
using System;

namespace EasyDo.RedisCache
{
    public class RedisDBCache : ICache, ISingletonDependency
    {

        private RedisDBUtility _redisDBUtility;
        public RedisDBCache(RedisDBUtility RedisDBUtility)
        {
            _redisDBUtility = RedisDBUtility;
        }
        public T Get<T>(string key)
        {
            using (var cilent = _redisDBUtility.GetRedisClient())
            {

                return cilent.Get<T>(key);
            }
        }

        public T Get<T>(string key, Func<T> func, int cacheTime=30)
        {
            using (var cilent = _redisDBUtility.GetRedisClient())
            {
                
                var res = cilent.Get<T>(key);
                if (res != null)
                {
                    return res;
                }

                var value = func();
                Set<T>(key, value, cacheTime);

                return value;
            }
        }

        public bool Remove(string key)
        {
            using (var cilent = _redisDBUtility.GetRedisClient())
            {
                return cilent.Remove(key);
            }
        }

        public long Increment(string key, uint value)
        {
            using (var cilent = _redisDBUtility.GetRedisClient())
            {
                return cilent.Increment(key, value);
            }
        }

        public long Decrement(string key, uint value)
        {
            using (var cilent = _redisDBUtility.GetRedisClient())
            {
                return cilent.Decrement(key, value);
            }
        }

        public bool Set<T>(string key, T data, int cacheTime=30)
        {
            using (var cilent = _redisDBUtility.GetRedisClient())
            {
                if (cacheTime > 0)
                {
                    return cilent.Set<T>(key, data, DateTime.Now.AddMinutes(cacheTime));
                }
                return cilent.Set<T>(key, data, DateTime.Now.AddMinutes(60));
            }
        }
    }
}
