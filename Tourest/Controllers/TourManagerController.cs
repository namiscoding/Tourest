using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tourest.Data;
using Tourest.Data.Entities;
using Tourest.Services;
using Tourest.ViewModels.Tour;

namespace Tourest.Controllers
{
    public class TourManagerController : Controller
    {
        private readonly ITourManagerService _tourManagerService;
        private readonly ApplicationDbContext _context;
        public TourManagerController(ApplicationDbContext context, ITourManagerService tourManagerService)
        {
            _context = context;
            _tourManagerService = tourManagerService;
        }

        public List<TourGuideListViewModel> TourGuides { get; set; }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TourGuide()
        {
            var tourGuides = _tourManagerService.GetAllTourGuides();
            return View("TourGuide", tourGuides);
        }
        public IActionResult IndexDetail()
        {
            return View();
        }

        public IActionResult TourGuideDetail(int id)
        {
            var tourGuide = _tourManagerService.GetDetail(id);
            if (tourGuide == null)
            {
                return NotFound();
            }
            return View("TourGuideDetail", tourGuide);
        }
        public async Task<IActionResult> TourGuideFeedback(int id)
        {
            var feedbacks = await _tourManagerService.GetFeedbacksByTourGuideIdAsync(id);

            if (feedbacks == null || !feedbacks.Any())
            {
                return NotFound();
            }

            return View(feedbacks);
        }
        public async Task<IActionResult> ViewCustomers(int tourId)
        {
            var customers = await _tourManagerService.GetCustomersForTourAsync(tourId);
            return View(customers);
        }
        public IActionResult ListTour()
        {
            var tours = _tourManagerService.GetAllTours();
            return View(tours);
        }
        public async Task<IActionResult> GetTourDetails(int id)
        {
            var tour = await _tourManagerService.GetTourDetailsAsync(id);
            if (tour == null)
            {
                return NotFound();
            }
            return View(tour); 
        }
        public async Task<IActionResult> CreateTour(TourListViewModel tourViewModel, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(tourViewModel);
            }

            try
            {
                if (imageFile != null && Path.GetExtension(imageFile.FileName).ToLower() != ".png")
                {
                    ModelState.AddModelError("ImageFile", "Only PNG files are allowed.");
                    return View(tourViewModel);
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/tours");
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(folderPath, fileName);
                     
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                  
                    tourViewModel.ImageUrls = $"/images/tours/{fileName}"; 
                    Debug.WriteLine($"File saved successfully at: {filePath}");
                }
                await _tourManagerService.CreateTourAsync(tourViewModel);
                return RedirectToAction(nameof(ListTour));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while saving the file: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the file. Please try again.");
                return View(tourViewModel);
            }
        }
        public async Task<IActionResult> EditTour(int id, TourListViewModel tourViewModel)
        {
            if (id != tourViewModel.TourId || !ModelState.IsValid)
            {
                return View(tourViewModel);
            }

            await _tourManagerService.EditTourAsync(tourViewModel);
            return RedirectToAction(nameof(ListTour));
        }

        public async Task<IActionResult> DeleteTour(int TourID)
        {
            Console.WriteLine($"Received TourID: {TourID}"); // Log the value for debugging

            if (TourID == 0)
            {
                TempData["ErrorMessage"] = $"Invalid TourID received: {TourID}.";
                return RedirectToAction(nameof(ListTour)); // Redirect back to ListTour with an error message
            }

            try
            {
                await _tourManagerService.RemoveTourAsync(TourID);
                TempData["SuccessMessage"] = "Tour deleted successfully."; // Store success message
                return RedirectToAction(nameof(ListTour)); // Redirect back to ListTour on success
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}"; // Store error message
                return RedirectToAction(nameof(ListTour)); // Redirect back to ListTour with error message
            }
        }
    }
}
