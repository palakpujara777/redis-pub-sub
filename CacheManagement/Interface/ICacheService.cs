using System.Collections.Concurrent;

namespace CacheManagement.Interface
{
    public interface ICacheService
    {
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);

        Task<T> GetAsync<T>(string key);

        Task<bool> RemoveAsync(string key);

        Task<bool> ExistsAsync(string key);

        public IEnumerable<string> GetAllKeysWithPrefix(string prefix);

        Task<bool> SetFieldAsync(string mainKey, string field, string value);

        Task<ConcurrentDictionary<string, string>> GetHashAsync(string mainKey);

        Task<string> GetFieldAsync(string mainKey, string field);

        Task<bool> RemoveFieldFromHashAsync(string mainKey, string field);

        Task<bool> HashFieldExistsAsync(string mainKey, string field);

        Task<bool> RemoveHashAsync(string mainKey);

        Task<bool> PublishAsync(string channel, string message);
    }
}
