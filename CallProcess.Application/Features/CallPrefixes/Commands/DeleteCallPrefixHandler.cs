using CacheManagement.Interface;
using CallProcess.Domain.Common;

namespace CallProcess.Application.Features.CallPrefixes.Commands
{
    public class DeleteCallPrefixHandler
    {
        private readonly ICacheService _cacheService;

        public DeleteCallPrefixHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<bool> Handle(DeleteCallPrefixCommand command)
        {
            /***** Delete country code from hash *****/
            string key = CacheHelper.CountryCodePrefix + command.Code;
            await _cacheService.RemoveFieldFromHashAsync(CacheHelper.CountryCodeKey, command.Code);

            /***** Delete country details from cache *****/
            await _cacheService.RemoveAsync(key);

            /***** Publish message of delete *****/
            await _cacheService.PublishAsync(CacheHelper.DeletePublishChannel, command.Code);

            return true;
        }
    }
}