using System;

namespace EasyDo.Dependency
{
    [ AttributeUsage(AttributeTargets.Class,AllowMultiple =true)]
    public  class AspectAttribute: Attribute
    {
        public  Type InterceptorType { get; set; }

        public AspectAttribute(Type InterceptorType)
        {
            this.InterceptorType = InterceptorType;
        }
    }
}
