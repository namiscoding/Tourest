using Tourest.Data.Entities;
using Tourest.Data.Repositories;
using Tourest.ViewModels.SupportRequest;

namespace Tourest.Services
{
    public class SupportRequestService : ISupportRequestService
    {
        private readonly ISupportRequestRepository _supportRequestRepository;

        public SupportRequestService(ISupportRequestRepository supportRequestRepository)
        {
            _supportRequestRepository = supportRequestRepository;
        }

        public async Task CreateSupportRequestAsync(CreateSupportRequestViewModel model, int customerId)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            // Không cần kiểm tra customerId <= 0 nữa vì Controller đã gán cứng là 4

            var newRequest = new SupportRequest
            {
                CustomerID = customerId, // Giờ sẽ luôn là 4 khi gọi từ Controller trên
                Subject = model.Subject,
                Message = model.Message,
                SubmissionDate = DateTime.UtcNow,
                Status = "Submitted",
                HandlerUserID = 1 // === GÁN CỨNG HandlerUserID LÀ 1 ===
                // ResolutionNotes ban đầu là null
            };

            await _supportRequestRepository.AddAsync(newRequest);

            
        }

        public async Task<MySupportRequestsViewModel> GetMyRequestsViewModelAsync(int customerId)
        {
            var requests = await _supportRequestRepository.GetByCustomerIdAsync(customerId);

            var viewModel = new MySupportRequestsViewModel
            {
                Requests = requests.Select(r => new SupportRequestSummaryViewModel
                {
                    RequestId = r.RequestID,
                    Subject = r.Subject,
                    Message = r.Message,
                    SubmissionDate = r.SubmissionDate,
                    Status = r.Status,
                    ResolutionNotes = r.ResolutionNotes
                }).ToList(),

                // Khởi tạo một đối tượng rỗng cho form tạo mới
                NewRequest = new CreateSupportRequestViewModel()
            };
            return viewModel;
        }
    }
}
