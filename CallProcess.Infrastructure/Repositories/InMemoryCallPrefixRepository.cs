using System.Collections.Concurrent;
using CallProcess.Application.Interfaces;
using CallProcess.Domain.Entities.CallPrefix;

namespace CallProcess.Infrastructure.Repositories
{
    public class InMemoryCallPrefixRepository : ICallPrefixRepository
    {
        private readonly ConcurrentDictionary<string, string> _prefixes = new();

        public InMemoryCallPrefixRepository()
        {
            _prefixes.TryAdd("91", "India");
            _prefixes.TryAdd("1", "USA");
            _prefixes.TryAdd("44", "UK");
        }

        public Task<IEnumerable<CallPrefixDetails>> GetAllAsync()
        {
            var result = _prefixes.Select(p => new CallPrefixDetails { Code = p.Key, Country = p.Value });
            return Task.FromResult(result);
        }

        public Task<CallPrefixDetails?> GetByPrefixAsync(string prefix)
        {
            if (_prefixes.TryGetValue(prefix, out var country))
            {
                return Task.FromResult<CallPrefixDetails?>(new CallPrefixDetails { Code = prefix, Country = country });
            }

            return Task.FromResult<CallPrefixDetails?>(null);
        }

        public Task AddOrUpdateAsync(CallPrefixDetails callPrefix)
        {
            _prefixes.AddOrUpdate(callPrefix.Code, callPrefix.Country, (_, _) => callPrefix.Country);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(string prefix)
        {
            return Task.FromResult(_prefixes.TryRemove(prefix, out _));
        }
    }
}
