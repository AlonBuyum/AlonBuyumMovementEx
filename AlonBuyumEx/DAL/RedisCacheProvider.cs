
using AlonBuyumEx.Services;

using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

using StackExchange.Redis;

namespace AlonBuyumEx.DAL
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheProvider(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }
       
        public object GetCache()
        {
            return _connectionMultiplexer;
        }
    }
}
