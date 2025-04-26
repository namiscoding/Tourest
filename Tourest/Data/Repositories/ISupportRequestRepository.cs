using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface ISupportRequestRepository
    {
        Task AddAsync(SupportRequest request);
        Task<IEnumerable<SupportRequest>> GetByCustomerIdAsync(int customerId);
    }
}
