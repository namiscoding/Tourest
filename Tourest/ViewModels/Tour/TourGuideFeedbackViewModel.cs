namespace Tourest.ViewModels.Tour
{
    public class TourGuideFeedbackViewModel
    {
        public int TourGuideUserID { get; set; }
        public decimal RatingValue { get; set; }
        public string Comment { get; set; }
        public string CustomerName { get; set; }
        public DateTime RatingDate { get; set; }
    }

}
