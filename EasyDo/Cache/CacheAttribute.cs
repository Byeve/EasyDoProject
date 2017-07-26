using EasyDo.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDo.Cache
{
    public class CacheAttribute : AspectAttribute
    {
        public int CacheTime { get; set; }

        public CacheAttribute(int cacheTime=30) 
            : base(typeof(CacheInterceptor))
        {
        }
    }
}
