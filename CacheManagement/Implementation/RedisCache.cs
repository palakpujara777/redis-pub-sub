using System.Collections.Concurrent;
using System.Text.Json;
using CacheManagement.Interface;
using StackExchange.Redis;

namespace CacheManagement.Implementation
{
    internal class RedisCache : ICacheService
    {
        #region Private Variables

        private readonly IConnectionMultiplexer _connectionMultiplexer;

        private readonly IDatabase _database;

        #endregion

        #region Constructor

        public RedisCache(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _database = _connectionMultiplexer.GetDatabase();

            _connectionMultiplexer.ConnectionFailed += (sender, args) =>
            {
                Console.WriteLine($"Redis connection failed: {args.Exception?.Message}");
            };

            _connectionMultiplexer.ConnectionRestored += (sender, args) =>
            {
                Console.WriteLine("Redis connection restored.");
            };
        }

        #endregion

        #region String Methods

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                /***** Set hash key in redis *****/
                var jsonData = JsonSerializer.Serialize(value);
                return await _database.StringSetAsync(key, jsonData, expiry);
            }
            catch
            {
                throw;
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                /***** Get value by key from redis for string collection *****/
                var jsonData = await _database.StringGetAsync(key);

                if (jsonData.IsNullOrEmpty)
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                /***** Delete key from redis *****/
                return await _database.KeyDeleteAsync(key);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                /***** Check if key exists in redis *****/
                return await _database.KeyExistsAsync(key);
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<string> GetAllKeysWithPrefix(string prefix)
        {
            try
            {
                var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
                var keys = server.Keys(pattern: $"{prefix}*").ToArray(); // Get all keys starting with the given prefix

                return keys.Select(key => key.ToString()); // Convert Redis keys to string
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region Hash Methods

        // Store data in a Redis hash (use a main key and hash fields)
        public async Task SetHashAsync(string mainKey, Dictionary<string, string> fields)
        {
            try
            {
                var hashEntries = fields.Select(f => new HashEntry(f.Key, f.Value)).ToArray();

                // Store the hash under the given main key
                await _database.HashSetAsync(mainKey, hashEntries);
            }
            catch
            {
                throw;
            }
        }

        // Set a single field value in the Redis hash
        public async Task<bool> SetFieldAsync(string mainKey, string field, string value)
        {
            try
            {
                return await _database.HashSetAsync(mainKey, field, value);
            }
            catch
            {
                throw;
            }
        }

        // Retrieve data from a Redis hash
        public async Task<ConcurrentDictionary<string, string>> GetHashAsync(string mainKey)
        {
            try
            {
                var hashEntries = await _database.HashGetAllAsync(mainKey);

                if (hashEntries.Length == 0)
                    return null; // No data found

                var result = new ConcurrentDictionary<string, string>(
                    hashEntries.Select(x => new KeyValuePair<string, string>(x.Name.ToString(), x.Value.ToString()))
                );

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> GetFieldAsync(string mainKey, string field)
        {
            try
            {
                return await _database.HashGetAsync(mainKey, field);
            }
            catch
            {
                throw;
            }
        }

        // Remove specific field from a Redis hash
        public async Task<bool> RemoveFieldFromHashAsync(string mainKey, string field)
        {
            try
            {
                return await _database.HashDeleteAsync(mainKey, field);
            }
            catch
            {
                throw;
            }
        }

        // Check if a field exists in the Redis hash
        public async Task<bool> HashFieldExistsAsync(string mainKey, string field)
        {
            try
            {
                return await _database.HashExistsAsync(mainKey, field);
            }
            catch
            {
                throw;
            }
        }

        // Remove the entire Redis hash (delete the key)
        public async Task<bool> RemoveHashAsync(string mainKey)
        {
            try
            {
                return await _database.KeyDeleteAsync(mainKey);
            }
            catch
            {
                throw;
            }
        }

        #endregion

        public async Task<bool> PublishAsync(string channel, string message)
        {
            try
            {
                var subscriber = _connectionMultiplexer.GetSubscriber();
                await subscriber.PublishAsync(channel, message);
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
