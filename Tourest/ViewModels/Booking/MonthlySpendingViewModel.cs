namespace Tourest.ViewModels.Booking
{
    public class MonthlySpendingViewModel
    {
        public string MonthYearLabel { get; set; } = string.Empty; 
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalAmount { get; set; }
    }
}
