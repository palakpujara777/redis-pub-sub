using CacheManagement.Interface;
using CallProcess.Domain.Common;

namespace CallProcess.Application.Features.CallPrefixes.Queries
{
    public class GetCallPrefixByCodeHandler
    {
        private readonly ICacheService _cacheService;

        public GetCallPrefixByCodeHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<string?> Handle(GetCallPrefixByCodeQuery query)
        {
            return await _cacheService.GetFieldAsync(CacheHelper.CountryCodeKey, query.Code);
        }
    }
}