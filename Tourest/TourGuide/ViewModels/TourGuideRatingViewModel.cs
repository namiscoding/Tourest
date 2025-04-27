using System;
using System.Collections.Generic;

namespace Tourest.TourGuide.ViewModels
{
    public class TourGuideRatingViewModel
    {
        // Assignment information
        public int AssignmentId { get; set; }
        public int TourGuideId { get; set; }
        public string TourGroupName { get; set; }
        public DateTime DepartureDate { get; set; }
        public string PickupPoint { get; set; }
        public int TotalAdults { get; set; }
        public int TotalChildren { get; set; }
        public int TotalPeople => TotalAdults + TotalChildren;
        public string TourName { get; set; }
        public string Status { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string Destination { get; set; }
        public string Description { get; set; }
        public DateTime CompletedDate { get; set; }
        public string ImageUrl { get; set; }

        // Rating statistics
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public RatingDistribution RatingDistribution { get; set; }

        // Detailed ratings and comments
        public List<RatingDetail> Ratings { get; set; } = new List<RatingDetail>();

        // Helper properties for UI
        public bool HasRatings => Ratings.Any();
        public string RatingSummary => $"{AverageRating:0.0} from {TotalRatings} reviews";
    }

    public class RatingDetail
    {
        public int RatingId { get; set; }
        public decimal RatingValue { get; set; }
        public string Comment { get; set; }
        public DateTime RatingDate { get; set; }

        // Customer information
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAvatar { get; set; }

        // Guide response
        public string GuideResponse { get; set; }
        public DateTime? ResponseDate { get; set; }

        // UI helpers
        public bool HasComment => !string.IsNullOrEmpty(Comment);
        public bool HasGuideResponse => !string.IsNullOrEmpty(GuideResponse);
        public string TimeAgo => GetTimeAgo(RatingDate);

        private string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.Now - date;

            if (timeSpan.TotalDays > 365)
                return $"{(int)(timeSpan.TotalDays / 365)} years ago";
            if (timeSpan.TotalDays > 30)
                return $"{(int)(timeSpan.TotalDays / 30)} months ago";
            if (timeSpan.TotalDays > 7)
                return $"{(int)(timeSpan.TotalDays / 7)} weeks ago";
            if (timeSpan.TotalDays > 1)
                return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalHours > 1)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalMinutes > 1)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";

            return "just now";
        }
    }

    public class RatingDistribution
    {
        public int FiveStar { get; set; }
        public int FourStar { get; set; }
        public int ThreeStar { get; set; }
        public int TwoStar { get; set; }
        public int OneStar { get; set; }

        public int Total => FiveStar + FourStar + ThreeStar + TwoStar + OneStar;

        public decimal FiveStarPercentage => Total > 0 ? (decimal)FiveStar / Total * 100 : 0;
        public decimal FourStarPercentage => Total > 0 ? (decimal)FourStar / Total * 100 : 0;
        public decimal ThreeStarPercentage => Total > 0 ? (decimal)ThreeStar / Total * 100 : 0;
        public decimal TwoStarPercentage => Total > 0 ? (decimal)TwoStar / Total * 100 : 0;
        public decimal OneStarPercentage => Total > 0 ? (decimal)OneStar / Total * 100 : 0;
    }
}