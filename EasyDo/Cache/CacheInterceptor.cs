using Castle.Core.Internal;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using System;

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
			var cacheAttribute = invocation.MethodInvocationTarget.GetAttribute<CacheAttribute>();

			if (cacheAttribute == null)
			{
				invocation.Proceed();
				return;
			}
			var cacheKey = string.Concat(invocation.Method.Name, JsonConvert.SerializeObject(invocation.Arguments));

			var value = cache.Get<string>(cacheKey, () => 
            {
			    invocation.Proceed();
				return SerializeWithType(invocation.ReturnValue, invocation.Method.ReturnType);
			}
			,cacheAttribute.CacheTime);

			if (invocation.ReturnValue == null)
			{
				invocation.ReturnValue = DeserializeWithType(value);
			}
		}

		
		private string SerializeWithType(object obj, Type type)
		{
			var serialized = JsonConvert.SerializeObject(obj);
			return string.Format(
				"{0}{1}{2}",
				type.AssemblyQualifiedName,
				TypeSeperator,
				serialized
				);
		}
		private object DeserializeWithType(string serializedObj)
		{
			var typeSeperatorIndex = serializedObj.IndexOf(TypeSeperator);
			var type = Type.GetType(serializedObj.Substring(0, typeSeperatorIndex));
			var serialized = serializedObj.Substring(typeSeperatorIndex + 1);
			return JsonConvert.DeserializeObject(serialized, type);
		}
	}
}
