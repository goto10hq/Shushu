using Microsoft.Extensions.Caching.Memory;
using Shushu.Attributes;
using Shushu.Tokens;
using System;
using System.Collections.Generic;
using System.Reflection;
using Sushi2;
using Microsoft.Spatial;
using System.Linq;
using Microsoft.Azure.Search.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;

namespace Shushu
{
    // TODO: force definition of index field ID?
    // TODO: check types when mapping?
    public class MapperCore
    {
        static readonly Lazy<MapperCore> _lazyInstance = new Lazy<MapperCore>(() => new MapperCore());
        public static MapperCore Instance => _lazyInstance.Value;

        Lazy<IMemoryCache> _classCache = new Lazy<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));
        public IMemoryCache ClassCache => _classCache.Value;

        Lazy<IMemoryCache> _propertiesCache = new Lazy<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));
        public IMemoryCache PropertiesCache => _propertiesCache.Value;        

        MapperCore()
        {
        }
    }

    public static class Mapper
    {
        public static AzureSearch MapIndex<T>(this T obj) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // get mappings
            var classMappings = GetClassMappings<T>();
            var propertyMappins = GetPropertyMappings<T>();            

            // create azure search instance
            var search = new AzureSearch();

            // set class mappings
            foreach (var cm in classMappings)
            {
                search.GetType().GetTypeInfo().GetProperty(cm.IndexField.ToString()).SetValue(search, cm.Value);
            }

            // set property mappings
            foreach (var pm in propertyMappins)
            {
                var value = obj.GetPropertyValue(pm.Property);

                if (value != null)
                {
                    if (pm.IndexField == Enums.IndexField.Point0 &&
                        value is GeoPoint gp)
                    {
                        value = GeographyPoint.Create(gp.Coordinates[0], gp.Coordinates[1]);
                        search.GetType().GetTypeInfo().GetProperty(pm.IndexField.ToString()).SetValue(search, value);
                    }
                    else
                    {
                        search.GetType().GetTypeInfo().GetProperty(pm.IndexField.ToString()).SetValue(search, value);
                    }
                }
            }

            return search;
        }

        public static SearchParameters MapSearchParameters<T>(this SearchParameters searchParameters) where T : class
        {
            var propertyMappings = GetPropertyMappings<T>();
            var classMappings = GetClassMappings<T>();           

            searchParameters.Select = searchParameters.Select.ToParameters().ToQuery(searchParameters.Select, propertyMappings);
            searchParameters.SearchFields = searchParameters.SearchFields.ToParameters().ToQuery(searchParameters.SearchFields, propertyMappings);
            searchParameters.HighlightFields = searchParameters.HighlightFields.ToParameters().ToQuery(searchParameters.HighlightFields, propertyMappings);
            searchParameters.Facets = searchParameters.Facets.ToParameters().ToQuery(searchParameters.Facets, propertyMappings);
            searchParameters.Filter = searchParameters.Filter.ToParameters().ToQuery(searchParameters.Filter, propertyMappings);
            searchParameters.OrderBy = searchParameters.OrderBy.ToParameters().ToQuery(searchParameters.OrderBy, propertyMappings);
            searchParameters.ScoringParameters = searchParameters.ScoringParameters.ToParameters().ToQuery(searchParameters.ScoringParameters, propertyMappings);            

            return searchParameters;
        }

        static IList<string> ToQuery(this IEnumerable<string> parameters, IList<string> query, IEnumerable<PropertyMapping> propertyMappins)
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

        static IList<ScoringParameter> ToQuery(this IEnumerable<string> parameters, IList<ScoringParameter> query, IEnumerable<PropertyMapping> propertyMappins)
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

        static string ToQuery(this IEnumerable<string> parameters, string query, IEnumerable<PropertyMapping> propertyMappins)
        {
            if (!parameters.Any())
                return query;

            foreach(var parameter in parameters)
            {
                var fi = propertyMappins.FirstOrDefault(pm => ("@" + pm.Property).Equals(parameter));

                if (fi == null)
                    throw new Exception($"No mapping defined for parameter {parameter}.");

                query = query.Replace(parameter, fi.IndexField.ToString().ToCamelCase());
            }

            return query;
        }

        static IEnumerable<string> ToParameters(this string query)
        {
            var result = new List<string>();

            if (query == null)
                return result;
            
            var regex = new Regex(@"(?<p>\@\w+)");
            var matches = regex.Matches(query);

            foreach(Match match in matches)
            {                
                result.Add(match.Groups["p"].Value);
            }

            return result;
        }

        static IEnumerable<string> ToParameters(this IEnumerable<ScoringParameter> parameters)
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

        static IEnumerable<string> ToParameters(this IEnumerable<string> parameters)
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

        static IEnumerable<ClassMapping> GetClassMappings<T>() where T: class
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

        static IEnumerable<PropertyMapping> GetPropertyMappings<T>() where T: class
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

        internal static string ToCamelCase(this string propertyName)
        {
            var resolver = new CamelCasePropertyNamesContractResolver();
            return resolver.GetResolvedPropertyName(propertyName);
        }        
    }
}
