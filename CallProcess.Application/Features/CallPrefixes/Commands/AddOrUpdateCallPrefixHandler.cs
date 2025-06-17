using CacheManagement.Interface;
using CallProcess.Domain.Common;
using CallProcess.Domain.Entities.CallPrefix;

namespace CallProcess.Application.Features.CallPrefixes.Commands
{
    public class AddOrUpdateCallPrefixHandler
    {
        private readonly ICacheService _cacheService;

        public AddOrUpdateCallPrefixHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task Handle(AddOrUpdateCallPrefixCommand command)
        {
            /***** Add code in country code list as hash *****/
            var prefix = new CallPrefixDetails { Code = command.Code, Country = command.Country };
            await _cacheService.SetFieldAsync(CacheHelper.CountryCodeKey, command.Code, command.Country);

            /***** Add country details as single key in cache *****/
            string key = CacheHelper.CountryCodePrefix + command.Code;
            await _cacheService.SetAsync<CallPrefixDetails>(key, prefix);

            /***** Publish message of add or update *****/
            await _cacheService.PublishAsync(CacheHelper.UpdatePublishChannel, command.Code);
        }
    }
}