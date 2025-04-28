namespace Tourest.ViewModels.SupportRequest
{
    public class SupportRequestSummaryViewModel
    {

        public int RequestId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty; // Hiển thị toàn bộ message theo yêu cầu
        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ResolutionNotes { get; set; }

    }
}
