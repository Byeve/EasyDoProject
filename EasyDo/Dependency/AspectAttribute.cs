using System;

namespace EasyDo.Dependency
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple =true)]
    public  abstract class AspectAttribute: Attribute
    {
        public  Type InterceptorType { get; set; }

        public AspectAttribute(Type InterceptorType)
        {
            this.InterceptorType = InterceptorType;
        }
    }
}
