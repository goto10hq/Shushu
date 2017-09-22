using Microsoft.Extensions.Caching.Memory;
using Shushu.Attributes;
using Shushu.Tokens;
using System;
using System.Collections.Generic;
using System.Reflection;
using Sushi2;
using Microsoft.Spatial;

namespace Shushu
{
    public class MapperCore
    {
        static readonly Lazy<MapperCore> _lazyInstance = new Lazy<MapperCore>(() => new MapperCore());
        public static MapperCore Instance => _lazyInstance.Value;

        Lazy<IMemoryCache> _classCache = new Lazy<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));
        public IMemoryCache ClassCache => _classCache.Value;

        Lazy<IMemoryCache> _propsCache = new Lazy<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));
        public IMemoryCache PropsCache => _propsCache.Value;

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
                        classMappings.Add(ca2);
                    }
                }

                MapperCore.Instance.ClassCache.Set(obj.GetType(), classMappings);
            }

            // get properties mappins
            var propsMappings = MapperCore.Instance.ClassCache.Get(obj.GetType()) as List<PropertyMapping>;

            if (propsMappings == null)
            {
                var props = obj.GetType().GetProperties();
                propsMappings = new List<PropertyMapping>();

                foreach (var p in props)
                {
                    var cac = p.GetCustomAttributes(false);

                    foreach (var ca in cac)
                    {
                        if (ca is PropertyMapping pm)
                        {
                            pm.Property = p.Name;

                            propsMappings.Add(pm);
                        }
                    }
                }


                MapperCore.Instance.PropsCache.Set(obj.GetType(), propsMappings);
            }

            // create azure search instance
            var search = new AzureSearch();

            // set class mappings
            foreach (var cm in classMappings)
            {
                search.GetType().GetTypeInfo().GetProperty(cm.IndexField.ToString()).SetValue(search, cm.Value);
            }

            // set property mappings
            foreach (var pm in propsMappings)
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
