using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ParallelPipesIntervals.Core
{
    public static class Funcs
    {
        public static bool IsPropertyOrMethodExist<T>(T settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>) settings).ContainsKey(name);
            return typeof(T).GetMethod(name) != null || settings.GetType().GetProperty(name) != null;
        }

        public static T Abs<T>(T value)
        {
            dynamic val = value;
            if (typeof(T) == typeof(double))
            {
                return Math.Abs(val);
            }

            return IsPropertyOrMethodExist(value, "Abs") ? val.Abs() : Math.Abs(val);
        }
        
        public static T Sqrt<T>(T value)
        {
            dynamic val = value;
            return IsPropertyOrMethodExist(value, "Sqrt") ? val.Sqrt() : Math.Sqrt(val);
        }

        public static object CreateInsGeneric(Type type, Type genericType) {
            Type[] typeArgs = { genericType };
            var ins = type.MakeGenericType(typeArgs);
            return Activator.CreateInstance(ins);
        }
    }
}