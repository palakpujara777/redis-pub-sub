using CallProcess.Domain.Entities.CallPrefix;

namespace CallProcess.Application.Interfaces
{
    public interface ICallPrefixRepository
    {
        Task<IEnumerable<CallPrefixDetails>> GetAllAsync();
        
        Task<CallPrefixDetails?> GetByPrefixAsync(string prefix);
        
        Task AddOrUpdateAsync(CallPrefixDetails callPrefix);
        
        Task<bool> DeleteAsync(string prefix);
    }
}
