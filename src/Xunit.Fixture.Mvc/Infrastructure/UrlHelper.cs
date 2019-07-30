using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xunit.Fixture.Mvc.Infrastructure
{
    internal static class UrlHelper
    {
        public static string GetWithQuery(string path, object query)
        {
            ICollection<string> GetValues(PropertyInfo p)
            {
                var value = p.GetValue(query);
                if (value == default)
                {
                    return Array.Empty<string>();
                }

                if (p.PropertyType == typeof(string))
                {
                    return new[] { value as string };
                }

                var type = p.PropertyType;
                if (type.IsArray && type.HasElementType || type.GetInterfaces().Any(i => i == typeof(IEnumerable)))
                {
                    var enumerable = (IEnumerable)value;
                    return enumerable.Cast<object>().Select(x => x.ToString()).ToList();
                }

                return new[] { value.ToString() };
            }

            var queryStrings = query.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => char.ToLower(p.Name[0]) + p.Name.Substring(1), GetValues)
                .Where(kvp => kvp.Value.Any())
                .SelectMany(kvp => kvp.Value.Select(v => $"{kvp.Key}={Uri.EscapeDataString(v)}"))
                .ToList();

            if (!queryStrings.Any())
            {
                return path;
            }

            return $"{path}?{string.Join("&", queryStrings)}";
        }
    }
}