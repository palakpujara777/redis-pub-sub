using CacheManagement;
using Microsoft.Extensions.DependencyInjection;

namespace CallProcess.Application
{
    public static class ApplicationExtension
    {
        public static bool InitializeApplication(this IServiceCollection services, string connectionString)
        {
            try
            {
                // Initializing cache service
                CacheExtension.InitializeCache(services, connectionString);

                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
