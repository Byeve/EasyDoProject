using EasyDo.Cache;
using EasyDo.Dependency;
using System;

namespace EasyDo.RedisCache
{
    public class RedisDbCache : ICache, ISingletonDependency
    {

        private readonly RedisDbUtility _redisDbUtility;
        public RedisDbCache(RedisDbUtility redisDbUtility)
        {
            _redisDbUtility = redisDbUtility;
        }
        public T Get<T>(string key)
        {
            using (var cilent = _redisDbUtility.GetRedisClient())
            {

                return cilent.Get<T>(key);
            }
        }

        public T Get<T>(string key, Func<T> func, int cacheTime=30)
        {
            using (var cilent = _redisDbUtility.GetRedisClient())
            {
                
                var res = cilent.Get<T>(key);
                if (res != null)
                {
                    return res;
                }

                var value = func();
                Set(key, value, cacheTime);

                return value;
            }
        }

        public bool Remove(string key)
        {
            using (var cilent = _redisDbUtility.GetRedisClient())
            {
                return cilent.Remove(key);
            }
        }

        public long Increment(string key, uint value)
        {
            using (var cilent = _redisDbUtility.GetRedisClient())
            {
                return cilent.Increment(key, value);
            }
        }

        public long Decrement(string key, uint value)
        {
            using (var cilent = _redisDbUtility.GetRedisClient())
            {
                return cilent.Decrement(key, value);
            }
        }

        public bool Set<T>(string key, T data, int cacheTime=30)
        {
            using (var cilent = _redisDbUtility.GetRedisClient())
            {
                return cacheTime > 0 ? cilent.Set(key, data, DateTime.Now.AddMinutes(cacheTime)) : cilent.Set(key, data, DateTime.Now.AddMinutes(60));
            }
        }
    }
}
