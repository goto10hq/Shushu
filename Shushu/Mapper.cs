using Microsoft.Extensions.Caching.Memory;
using Shushu.Tokens;
using System;
using System.Reflection;
using Sushi2;
using Microsoft.Spatial;
using Microsoft.Azure.Search.Models;
using System.Linq;

namespace Shushu
{
    // TODO: check types when mapping?
    internal class MapperCore
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
        public static AzureSearch MapToIndex<T>(this T obj) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // no mappings needed for AzureSearch objects
            if (typeof(T) == typeof(AzureSearch))
                return obj as AzureSearch;

            // get mappings
            var classMappings = MapperHelpers.GetClassMappings<T>();
            var propertyMappings = MapperHelpers.GetPropertyMappings<T>();

            // check if id mapping exists
            if (classMappings.All(x => x.IndexField != Enums.IndexField.Id) &&
               propertyMappings.All(x => x.IndexField != Enums.IndexField.Id))
            {
                throw new Exception("You have to define a mapping for index field Id.");
            }

            // create azure search instance
            var search = new AzureSearch();

            // set class mappings
            foreach (var cm in classMappings)
            {
                search.GetType().GetTypeInfo().GetProperty(cm.IndexField.ToString()).SetValue(search, cm.Value);
            }

            // set property mappings
            foreach (var pm in propertyMappings)
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

        public static SearchParameters MapToSearchParameters<T>(this SearchParameters searchParameters) where T : class
        {
            var propertyMappings = MapperHelpers.GetPropertyMappings<T>();
            var classMappings = MapperHelpers.GetClassMappings<T>();

            searchParameters.Select = searchParameters.Select.ToParameters().ToQuery(searchParameters.Select, propertyMappings);
            searchParameters.SearchFields = searchParameters.SearchFields.ToParameters().ToQuery(searchParameters.SearchFields, propertyMappings);
            searchParameters.HighlightFields = searchParameters.HighlightFields.ToParameters().ToQuery(searchParameters.HighlightFields, propertyMappings);
            searchParameters.Facets = searchParameters.Facets.ToParameters().ToQuery(searchParameters.Facets, propertyMappings);
            searchParameters.Filter = searchParameters.Filter.ToParameters().ToQuery(searchParameters.Filter, propertyMappings);
            searchParameters.OrderBy = searchParameters.OrderBy.ToParameters().ToQuery(searchParameters.OrderBy, propertyMappings);
            searchParameters.ScoringParameters = searchParameters.ScoringParameters.ToParameters().ToQuery(searchParameters.ScoringParameters, propertyMappings);

            return searchParameters;
        }
    }
}
