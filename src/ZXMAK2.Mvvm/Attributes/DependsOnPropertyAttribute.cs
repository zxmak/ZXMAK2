using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;


namespace ZXMAK2.Mvvm.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class DependsOnPropertyAttribute : Attribute
    {
        public DependsOnPropertyAttribute(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            PropertyName = propertyName;
        }


        public string PropertyName { get; private set; }


        #region Helpers

        private static readonly Dictionary<Type, IDictionary<string, IEnumerable<string>>> s_propertyTrees = new Dictionary<Type, IDictionary<string, IEnumerable<string>>>();

        public static IDictionary<string, IEnumerable<string>> GetDependencyTree(Type type)
        {
            IDictionary<string, IEnumerable<string>> result;
            if (s_propertyTrees.TryGetValue(type, out result))
            {
                return result;
            }
            result = new Dictionary<string, IEnumerable<string>>();
            var properties = type
                .GetProperties();
            result = properties
                .SelectMany(pi => GetAttributes(pi).Select(attr => new { Key = attr.PropertyName, Value = pi.Name }))
                .GroupBy(pair => pair.Key)
                .ToDictionary(group => group.Key, group => (IEnumerable<string>)group.Select(item => item.Value).ToArray());
            s_propertyTrees[type] = result;
            return result;
        }

        private static IEnumerable<DependsOnPropertyAttribute> GetAttributes(PropertyInfo pi)
        {
            return pi
                .GetCustomAttributes(typeof(DependsOnPropertyAttribute), false)
                .Cast<DependsOnPropertyAttribute>();
        }

        #endregion Helpers
    }
}
