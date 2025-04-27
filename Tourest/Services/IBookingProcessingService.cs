namespace Tourest.Services
{
    public interface IBookingProcessingService
    {
        Task<(int updatedCount, string message)> UpdateStatusesForPastDeparturesAsync(CancellationToken cancellationToken = default);
    }

}
