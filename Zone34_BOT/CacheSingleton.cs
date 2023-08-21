using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pale_BOT
{
    class CacheSingleton
    {
        private static MemoryCache? _cache;
        private static object _lock = new object();
        private static MemoryCacheOptions _memoryCacheOptions { get; } = JsonSerializer.Deserialize<MemoryCacheOptions>(File.ReadAllText("..\\..\\..\\Configs\\cacheOptions.cfg")) ?? new MemoryCacheOptions();
        private CacheSingleton() { }
        public static MemoryCacheEntryOptions GetStandartCacheEntryOptions()
        {
            return new MemoryCacheEntryOptions().SetSize(1).SetSlidingExpiration(TimeSpan.FromMinutes(1));
        }
        public static MemoryCache GetInstance()
        {
            if (_cache == null)
            {
                lock (_lock)
                {
                    if (_cache == null)
                        _cache = new MemoryCache(_memoryCacheOptions);
                }
            }
            return _cache;
            
        }
    }
}
