namespace Tourest.ViewModels.Admin.AdminTour
{
    public class DeleteTourConfirmationViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        // Có thể thêm cảnh báo nếu tour đang được sử dụng
        public bool IsInUse { get; set; }
    }
}
