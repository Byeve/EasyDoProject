using EasyDo.Dependency;

namespace EasyDo.Cache
{
    public class CacheAttribute : AspectAttribute
    {
        public int CacheTime { get; set; }

        public CacheAttribute(int cacheTime=30) 
            : base(typeof(CacheInterceptor))
        {
            CacheTime = cacheTime;
        }
    }
}
