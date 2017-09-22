using Microsoft.Extensions.Caching.Memory;
using Shushu.Attributes;
using Shushu.Tokens;
using System;
using System.Collections.Generic;
using System.Reflection;
using Sushi2;
using Microsoft.Spatial;
using System.Linq;

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
        public static AzureSearch MapToSearch<T>(this T obj) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // get class mappings
            var classMappings = MapperCore.Instance.ClassCache.Get(obj.GetType()) as List<ClassMapping>;

            if (classMappings == null)
            {
                var cac = obj.GetType().GetCustomAttributes(false);
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

                MapperCore.Instance.ClassCache.Set(obj.GetType(), classMappings);
            }

            // get properties mappins
            var propertyMappins = MapperCore.Instance.ClassCache.Get(obj.GetType()) as List<PropertyMapping>;

            if (propertyMappins == null)
            {
                var props = obj.GetType().GetProperties();
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

                MapperCore.Instance.PropertiesCache.Set(obj.GetType(), propertyMappins);
            }

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
    }
}
