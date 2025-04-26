namespace Tourest.ViewModels.Booking
{
    public class BookingHistoryItemViewModel
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string? PickupPoint { get; set; }
        public int TotalPrice { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CancellationDate { get; set; }
        public int? RefundAmount { get; set; }

        // Thêm TourId để tạo link đến chi tiết tour nếu muốn
        public int TourId { get; set; }
    }
}
