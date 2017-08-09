using System;

namespace EasyDo.Cache
{
    public interface ICache
    {
        T Get<T>(string key);
        T Get<T>(string key, Func<T> func, int cacheTime=30);
        bool Remove(string key);
        long Increment(string key, uint value);
        long Decrement(string key, uint value);
        bool Set<T>(string key, T data, int cacheTime=30);
    }
}
