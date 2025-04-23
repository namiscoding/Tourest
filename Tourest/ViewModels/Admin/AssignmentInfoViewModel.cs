namespace Tourest.ViewModels.Admin
{
    public class AssignmentInfoViewModel
    {
        public int AssignmentId { get; set; }
        public string TourName { get; set; } = "N/A"; // Tên Tour
        public DateTime? DepartureDate { get; set; } // Ngày khởi hành
        public string TourGuideName { get; set; } = "N/A"; // Tên HDV được gán
        public string AssignmentStatus { get; set; } = string.Empty; // Trạng thái phân công
        public DateTime AssignmentDate { get; set; } // Ngày phân công
    }
}
