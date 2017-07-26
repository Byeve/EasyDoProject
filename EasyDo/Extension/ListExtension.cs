using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EasyDo.Extension
{
    public static class ListExtension
    {
        public static List<string> AddPropertyNames(this List<string> propertyNames, params Expression<Func<object>>[] expressions)
        {
            foreach (var exp in expressions)
            {
                propertyNames.AddPropertyName(exp);
            }

            return propertyNames;
        }

        public static List<string> AddPropertyName<TProperty>(this List<string> propertyNames, Expression<Func<TProperty>> exp)
        {
            var member = exp.Body as MemberExpression;

            if (member != null)
            {
                var propertyName = member.Member.Name;
                propertyNames.Add(propertyName);
            }
            else
            {
                var s = exp.Body.ToString();
                var index = s.LastIndexOf(".", StringComparison.InvariantCulture);
                var length = s.Length - index - 2;
                if (index >= 0 && length > 0)
                {
                    var propertyName = s.Substring(index + 1, length);
                    propertyNames.Add(propertyName);
                }
            }
            return propertyNames;
        }
    }
}
