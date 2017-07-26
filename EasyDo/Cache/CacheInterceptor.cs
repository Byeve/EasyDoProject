using Castle.Core.Internal;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDo.Cache
{
    public class CacheInterceptor : IInterceptor
    {
        private readonly ICache cache;

        private const char TypeSeperator = '|';
        public CacheInterceptor(ICache cache)
        {
            this.cache = cache;
        }
        public void Intercept(IInvocation invocation)
        {
            //var cacheAttribute = invocation.MethodInvocationTarget.GetAttribute<CacheAttribute>();
            //if (cacheAttribute == null)
            //{
            //    invocation.Proceed();
            //    return;
            //}
            //var cacheKey = string.Concat(invocation.TargetType.FullName, ".", invocation.Method.Name, "(", JsonConvert.SerializeObject(invocation.Arguments), ")");
            //var obj = _Cache.Get<string>(cacheKey);
            //if (obj != null)
            //{
            //    invocation.ReturnValue = DeserializeWithType(obj);
            //    return;
            //}
            invocation.Proceed();
            //_Cache.Set<string>(cacheKey, SerializeWithType(invocation.ReturnValue, invocation.Method.ReturnType), cacheAttribute.CacheTime);
            //return;
        }
        //public string SerializeWithType(object obj, Type type)
        //{
        //    var serialized = JsonConvert.SerializeObject(obj);
        //    return string.Format(
        //        "{0}{1}{2}",
        //        type.AssemblyQualifiedName,
        //        TypeSeperator,
        //        serialized
        //        );
        //}
        //public object DeserializeWithType(string serializedObj)
        //{
        //    var typeSeperatorIndex = serializedObj.IndexOf(TypeSeperator);
        //    var type = Type.GetType(serializedObj.Substring(0, typeSeperatorIndex));
        //    var serialized = serializedObj.Substring(typeSeperatorIndex + 1);
        //    return JsonConvert.DeserializeObject(serialized, type);
        //}
    }
}
