using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Thêm using này
using System;
using Tourest.Data;
using Tourest.Data.Entities.Momo;
using Tourest.Services.Momo; // Thêm using này
// ... các using khác ...

namespace Tourest.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IMomoService _momoService;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CheckoutController> _logger; // Thêm logger

        public CheckoutController(IMomoService momoService, ApplicationDbContext dbContext, ILogger<CheckoutController> logger) // Inject logger
        {
            _momoService = momoService ?? throw new ArgumentNullException(nameof(momoService));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger; // Gán logger
        }

        
    }
}