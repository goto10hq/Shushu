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

            //var p = new SearchParameters
            //{
            //    Filter = "entity eq 'oslavin/object'",
            //    OrderBy = new List<string> { "order" },                
            //    Select = new List<string> {  "order" },
            //    Facets = new List<string> { "order" },
            //    HighlightFields = new List<string> {  "xxx" },
            //    ScoringParameters = new List<ScoringParameter> {  new ScoringParameter("x", new List<string> {  "a" })},
            //    SearchFields = new List<string> { "order" },                
            //};

            searchParameters.Filter = searchParameters.Filter.ToParameters().ToQuery(searchParameters.Filter, propertyMappings);            

            return searchParameters;
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

                query = query.Replace(parameter, fi.IndexField.ToString());
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
