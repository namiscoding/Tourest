using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tourest.Data;
using Tourest.Data.Entities;
using Tourest.Services;
using Tourest.ViewModels.Tour;

namespace Tourest.Controllers
{
    //[Authorize(Roles = "Admin,Manager")]
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
        [HttpGet]
        public async Task<IActionResult> EditTour(int id)
        {
            var tour = await _tourManagerService.GetTourDetailsAsync(id);
            if (tour == null)
            {
                TempData["ErrorMessage"] = "Tour not found.";
                return RedirectToAction(nameof(ListTour)); 
            }
            var tourViewModel = new TourListViewModel
            {
                TourId = tour.TourId,
                Name = tour.Name,
                Destination = tour.Destination,
                DurationDays = tour.DurationDays,
                DurationNights = tour.DurationNights,
                ChildPrice = tour.ChildPrice,
                AdultPrice = tour.AdultPrice,
                Description = tour.Description,
                Status = tour.Status,
                MinGroupSize = tour.MinGroupSize,
                MaxGroupSize = tour.MaxGroupSize,
                DeparturePoints = tour.DeparturePoints,
                ExcludedServices = tour.ExcludedServices,
                IncludedServices = tour.IncludedServices,
                ImageUrls = tour.ImageUrls,  
                IsCancellable = tour.IsCancellable,
                CancellationPolicyDescription = tour.CancellationPolicyDescription
            };

            return View(tourViewModel);  
        }
        [HttpPost]
        public async Task<IActionResult> EditTour(int id, TourListViewModel tourViewModel, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(tourViewModel);
            }
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    if (Path.GetExtension(imageFile.FileName).ToLower() != ".png")
                    {
                        ModelState.AddModelError("ImageFile", "Only PNG files are allowed.");
                        return View(tourViewModel);
                    }

                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/tours");
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    tourViewModel.ImageUrls = $"/images/tours/{fileName}";
                }

                await _tourManagerService.EditTourAsync(tourViewModel);

                TempData["SuccessMessage"] = "Tour updated successfully!";
                return RedirectToAction(nameof(ListTour));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(tourViewModel);
            }
        }

        public async Task<IActionResult> DeleteTour(int TourID)
        {
            Console.WriteLine($"Received TourID: {TourID}"); 

            if (TourID == 0)
            {
                TempData["ErrorMessage"] = $"Invalid TourID received: {TourID}.";
                return RedirectToAction(nameof(ListTour));  
            }

            try
            {
                await _tourManagerService.RemoveTourAsync(TourID);
                TempData["SuccessMessage"] = "Tour deleted successfully.";  
                return RedirectToAction(nameof(ListTour));  
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";  
                return RedirectToAction(nameof(ListTour));  
            }
        }
        public async Task<IActionResult> ViewSchedule(int id)
        {
            var schedule = await _tourManagerService.GetTourGuideScheduleAsync(id);
            ViewBag.TourGuideId = id; 
            return View(schedule);
        }

    }
}
