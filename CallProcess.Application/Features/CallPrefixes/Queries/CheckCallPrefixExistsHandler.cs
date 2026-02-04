using CacheManagement.Interface;
using CallProcess.Domain.Common;

namespace CallProcess.Application.Features.CallPrefixes.Queries
{
    public class CheckCallPrefixExistsHandler
    {
        private readonly ICacheService _cacheService;

        public CheckCallPrefixExistsHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<bool> Handle(CheckCallPrefixExistsQuery query)
        {
            return await _cacheService.HashFieldExistsAsync(CacheHelper.CountryCodeKey, query.Code);
        }
    }
}
