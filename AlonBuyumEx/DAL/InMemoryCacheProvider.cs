
using AlonBuyumEx.Services;

using Microsoft.Extensions.Caching.Memory;

namespace AlonBuyumEx.DAL
{
    public class InMemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _memoryCache;
       
        public InMemoryCacheProvider(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
      
        public object GetCache()
        {
            return _memoryCache;
        }
    }
}
