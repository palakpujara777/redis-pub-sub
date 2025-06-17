using CacheManagement.Implementation;
using CacheManagement.Interface;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CacheManagement
{
    public static class CacheExtension
    {
        public static bool InitializeCache(this IServiceCollection services, string connectionString)
        {
            try
            {
                // TODO add this in appsetting file of current library
                var cacheType = "Redis";

                if (string.IsNullOrEmpty(cacheType))
                {
                    throw new NotSupportedException("Cache type is not mentioned in confguration.");
                }

                switch (cacheType)
                {
                    case "Redis":
                        services.AddSingleton<ICacheService, RedisCache>();
                        ConnectToRedis(services, connectionString);
                        break;
                    default:
                        throw new NotSupportedException("Cache type is not supported.");
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        private static void ConnectToRedis(IServiceCollection services, string connectionString)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new NotSupportedException("Connection string should not be an empty.");
                }

                var configurationOptions = ConfigurationOptions.Parse(connectionString);

                IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
                services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
            }
            catch
            {
                throw;
            }
        }
    }
}
