using System;
using Microsoft.Extensions.Caching.Memory;

namespace Shushu
{
    /// <summary>
    /// Mapper core.
    /// </summary>
    class MapperCore
    {
        static readonly Lazy<MapperCore> _lazyInstance = new Lazy<MapperCore>(() => new MapperCore());
        public static MapperCore Instance => _lazyInstance.Value;

        Lazy<IMemoryCache> _classCache = new Lazy<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));
        public IMemoryCache ClassCache => _classCache.Value;

        Lazy<IMemoryCache> _propertiesCache = new Lazy<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));
        public IMemoryCache PropertiesCache => _propertiesCache.Value;
    }
}
