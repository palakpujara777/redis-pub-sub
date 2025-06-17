using CacheManagement.Interface;
using CallProcess.Domain.Common;
using CallProcess.Domain.Entities.CallPrefix;

namespace CallProcess.Application.Features.CallPrefixes.Queries
{
    public class GetAllCallPrefixesHandler
    {
        #region Private Variables

        private readonly ICacheService _cacheService;
        
        #endregion

        public GetAllCallPrefixesHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        #region Handler

        public async Task<IEnumerable<CallPrefixDetails>> Handle(GetAllCallPrefixesQuery query)
        {
            var result = new List<CallPrefixDetails>();

            var keys = _cacheService.GetAllKeysWithPrefix(CacheHelper.CountryCodePrefix);

            foreach (var key in keys)
            {
                var prefix = await _cacheService.GetAsync<CallPrefixDetails>(key);
                if (prefix != null)
                    result.Add(prefix);
            }

            return result;
        }

        #endregion
    }
}
