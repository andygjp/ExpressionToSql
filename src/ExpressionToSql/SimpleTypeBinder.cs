namespace ExpressionToSql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class SimpleTypeBinder
    {
        public Dictionary<string, PropertyInfo> GetProperties(Type t)
        {
            var properties = from prop in GetDeclaredProperties(t)
                where prop.CanWrite
                where IsSimpleType(prop.PropertyType)
                select prop;
            return properties.ToDictionary(x => x.Name);
        }

        private static IEnumerable<PropertyInfo> GetDeclaredProperties(Type t)
        {
#if NET45
            return t.GetRuntimeProperties();
#else
            return t.GetTypeInfo().DeclaredProperties;
#endif
        }

        private static bool IsSimpleType(Type t)
        {
            while (true)
            {
                if (IsPrimitive(t))
                {
                    return true;
                }
                if (t == typeof(decimal))
                {
                    return true;
                }
                if (t == typeof(string))
                {
                    return true;
                }
                t = Nullable.GetUnderlyingType(t);
                if (t == null)
                {
                    break;
                }
            }
            return false;
        }

        private static bool IsPrimitive(Type t)
        {
#if NET45
            return t.IsPrimitive;
#else
            return t.GetTypeInfo().IsPrimitive;
#endif
        }
    }
}