using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Serialization;
using Shushu.Attributes;

namespace Shushu
{
    /// <summary>
    /// Mapper helpers.
    /// </summary>
    static class MapperHelpers
    {
        static readonly object _lockProperties = new object();
        static readonly object _lockClass = new object();

        internal static IList<string> ToQuery(this IEnumerable<string> parameters, IList<string> query, IEnumerable<PropertyMapping> propertyMappins)
        {
            if (!parameters.Any())
                return query;

            var newQuery = query.ToList();

            foreach (var parameter in parameters)
            {
                var fi = propertyMappins.FirstOrDefault(pm => ("@" + pm.Property).Equals(parameter));

                if (fi == null)
                    throw new Exception($"No mapping defined for parameter {parameter}.");

                for (var i = 0; i < newQuery.Count; i++)
                {
                    newQuery[i] = newQuery[i].Replace(parameter, fi.IndexField.ToString().ToCamelCase());
                }
            }

            return newQuery;
        }

        internal static IList<ScoringParameter> ToQuery(this IEnumerable<string> parameters, IList<ScoringParameter> query, IEnumerable<PropertyMapping> propertyMappins)
        {
            if (!parameters.Any())
                return query;

            var newQuery = query.ToList();

            foreach (var parameter in parameters)
            {
                var fi = propertyMappins.FirstOrDefault(pm => ("@" + pm.Property).Equals(parameter));

                if (fi == null)
                    throw new Exception($"No mapping defined for parameter {parameter}.");

                for (var i = 0; i < newQuery.Count; i++)
                {
                    newQuery[i] = new ScoringParameter(newQuery[i].Name.Replace(parameter, fi.IndexField.ToString().ToCamelCase()), newQuery[i].Values);
                }
            }

            return newQuery;
        }

        internal static string ToQuery(this IEnumerable<string> parameters, string query, IEnumerable<PropertyMapping> propertyMappins)
        {
            if (!parameters.Any())
                return query;

            foreach (var parameter in parameters)
            {
                var fi = propertyMappins.FirstOrDefault(pm => ("@" + pm.Property).Equals(parameter));

                if (fi == null)
                    throw new Exception($"No mapping defined for parameter {parameter}.");

                query = query.Replace(parameter, fi.IndexField.ToString().ToCamelCase());
            }

            return query;
        }

        internal static IEnumerable<string> ToParameters(this string query)
        {
            var result = new List<string>();

            if (query == null)
                return result;

            var regex = new Regex(@"(?<p>\@\w+)");
            var matches = regex.Matches(query);

            foreach (Match match in matches)
            {
                result.Add(match.Groups["p"].Value);
            }

            return result;
        }

        internal static IEnumerable<string> ToParameters(this IEnumerable<ScoringParameter> parameters)
        {
            var result = new List<string>();

            if (parameters == null)
                return result;

            foreach (var parameter in parameters)
            {
                var regex = new Regex(@"(?<p>\@\w+)");
                var matches = regex.Matches(parameter.Name);

                foreach (Match match in matches)
                {
                    result.Add(match.Groups["p"].Value);
                }
            }

            return result;
        }

        internal static IEnumerable<string> ToParameters(this IEnumerable<string> parameters)
        {
            var result = new List<string>();

            if (parameters == null)
                return result;

            foreach (var parameter in parameters)
            {
                var regex = new Regex(@"(?<p>\@\w+)");
                var matches = regex.Matches(parameter);

                foreach (Match match in matches)
                {
                    result.Add(match.Groups["p"].Value);
                }
            }

            return result;
        }

        internal static IEnumerable<ClassMapping> GetClassMappings<T>() where T : class
        {
            lock (_lockClass)
            {
                var classMappings = MapperCore.Instance.ClassCache.Get(typeof(T)) as List<ClassMapping>;

                if (classMappings == null)
                {
                    var cac = typeof(T).GetCustomAttributes(false);
                    classMappings = new List<ClassMapping>();

                    foreach (var ca in cac)
                    {
                        if (ca is ClassMapping ca2)
                        {
                            if (classMappings.Any(cm => cm.IndexField == ca2.IndexField))
                                throw new Exception($"Multiple class mapping for index field ${ca2.IndexField}.");

                            classMappings.Add(ca2);
                        }
                    }

                    MapperCore.Instance.ClassCache.Set(typeof(T), classMappings);
                }

                return classMappings;
            }
        }

        internal static IEnumerable<PropertyMapping> GetPropertyMappings<T>() where T : class
        {
            lock (_lockProperties)
            {
                var propertyMappins = MapperCore.Instance.ClassCache.Get(typeof(T)) as List<PropertyMapping>;

                if (propertyMappins == null)
                {
                    var props = typeof(T).GetProperties();
                    propertyMappins = new List<PropertyMapping>();

                    foreach (var p in props)
                    {
                        var cac = p.GetCustomAttributes(false);

                        foreach (var ca in cac)
                        {
                            if (ca is PropertyMapping pm)
                            {
                                pm.Property = p.Name;

                                if (propertyMappins.Any(prop => prop.IndexField == pm.IndexField))
                                    throw new Exception($"Multiple property mapping for index field ${pm.IndexField}.");

                                propertyMappins.Add(pm);
                            }
                        }
                    }

                    MapperCore.Instance.PropertiesCache.Set(typeof(T), propertyMappins);
                }

                return propertyMappins;
            }
        }

        internal static string ToCamelCase(this string propertyName)
        {
            var resolver = new CamelCasePropertyNamesContractResolver();
            return resolver.GetResolvedPropertyName(propertyName);
        }
    }
}
