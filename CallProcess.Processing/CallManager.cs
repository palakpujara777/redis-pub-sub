using CallProcess.Application.Interfaces.CallProcessing;
using CacheManagement.Interface;
using StackExchange.Redis;
using System.Collections.Concurrent;
using CallProcess.Domain.Common;

namespace CallProcess.Processing
{
    public class CallManager : ICallManager
    {
        #region Properties

        private readonly ICacheService _cacheService;
       
        private readonly IConnectionMultiplexer _redis;
        
        public static ConcurrentDictionary<string, string> _inMemoryData;

        #endregion

        public CallManager(ICacheService cacheService, IConnectionMultiplexer redis)
        {
            _cacheService = cacheService;
            _redis = redis;
            _inMemoryData = new ConcurrentDictionary<string, string>();
        }

        #region Public Methods

        public async Task Init()
        {
            /***** Get all country code from cache *****/
            _inMemoryData = await _cacheService.GetHashAsync(CacheHelper.CountryCodeKey);

            Console.WriteLine("Inmemory data loaded successfully");
        }

        public async Task Start()
        {
            var subscriber = _redis.GetSubscriber();

            /***** Create subscriber to get latest country codes from cache when added *****/
            await subscriber.SubscribeAsync(CacheHelper.UpdatePublishChannel, async (channel, message) =>
            {
                try
                {
                    await UpdateMemoryWithLatestData(message);   
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Redis Subscriber Error] {ex.Message}");
                }
            });

            /***** Create subscriber to remove country code from in memory *****/
            await subscriber.SubscribeAsync(CacheHelper.DeletePublishChannel, async (channel, message) =>
            {
                try
                {
                    await RemoveCodeFromInMeory(message);   
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Redis Subscriber Error] {ex.Message}");
                }
            });

            Console.WriteLine("[CallManager] Subscription started.");
        }

        /***** Get country name by code from in memory *****/
        public string? GetByCode(string code)
        {
            _inMemoryData.TryGetValue(code, out string? result);
            return result;
        }

        #endregion

        #region Private Methods

        private async Task UpdateMemoryWithLatestData(string message)
        {
            try
            {
                /***** Get country name by code received from publisher *****/
                string countryName = await _cacheService.GetFieldAsync(CacheHelper.CountryCodeKey, message);
                _inMemoryData.TryAdd(message, countryName);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in updating in memory with latest data. " + ex.Message);
            }
        }
        
        private async Task RemoveCodeFromInMeory(string message)
        {
            try
            {
                /***** Get country name by code received from publisher *****/
                _inMemoryData.TryRemove(message, out string countryName);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in updating in memory with latest data. " + ex.Message);
            }
        }

        #endregion
    }
}