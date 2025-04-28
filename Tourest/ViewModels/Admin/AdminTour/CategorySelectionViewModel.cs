namespace Tourest.ViewModels.Admin.AdminTour
{
    public class CategorySelectionViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; } // Để đánh dấu checkbox
    }
}
