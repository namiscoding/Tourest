using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tourest.Data;
using Tourest.Data.Repositories;
using Tourest.Services;
using Tourest.Data.Entities.Momo;
using Tourest.TourGuide.Repositories;
using Tourest.TourGuide.Services;
using Tourest.Services.Momo;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Tourest.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Tourest.BackgroundServices;
using Tourest.Helpers;
using BCrypt.Net; // Thêm import này
using Microsoft.Extensions.Logging; // Add this


namespace Tourest
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // connect momo API
            builder.Services.Configure<Tourest.Services.Momo.MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
            builder.Services.AddScoped<IMomoService, MomoService>();

            // In program.cs or Startup.cs, add debug-level logging for MoMo-related services
            builder.Services.AddLogging(logging =>
            {
                logging.AddFilter("Tourest.Services.Momo", LogLevel.Debug);
                logging.AddFilter("Tourest.Controllers.PaymentController", LogLevel.Debug);
                // Add console for development environment
                logging.AddConsole();
                logging.AddDebug();
            });

            // Lấy connection string từ appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Đăng ký ApplicationDbContext với DI Container và chỉ định dùng SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddIdentityApiEndpoints<IdentityUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();


            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(options =>
              {
                  options.LoginPath = "/Authentication/Login";
                  options.AccessDeniedPath = "/Authentication/AccessDenied";
              });
            builder.Services.AddAuthorization();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Authentication/Login";

                options.SlidingExpiration = true;
            });
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian timeout của session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddScoped<ITourRepository, TourRepository>();
            builder.Services.AddScoped<ITourService, TourService>();

            builder.Services.AddScoped<ITourManagerRepository, TourManagerRepository>();
            builder.Services.AddScoped<ITourManagerService, TourManagerService>();
            builder.Services.AddScoped<TourManagerRepository>();

            builder.Services.AddScoped<ITourAssignmentService, TourAssignmentService>();
            builder.Services.AddScoped<IAssignedTourRespo, AssignedTourRepository>();

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<ITourGuideService, TourGuideService>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IAccountService, AccountService>();

            builder.Services.AddScoped<ITourAssignmentService, TourAssignmentService>();
            builder.Services.AddScoped<IAssignedTourRespo, AssignedTourRepository>();

            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.AddScoped<IPhotoService, CloudinaryPhotoService>();

            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            builder.Services.AddScoped<ISupportRequestRepository, SupportRequestRepository>();
            builder.Services.AddScoped<ISupportRequestService, SupportRequestService>();

            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            builder.Services.AddScoped<ITourGroupRepository, TourGroupRepository>();

            builder.Services.AddScoped<IRatingRepository, RatingRepository>();
            builder.Services.AddScoped<ITourRatingRepository, TourRatingRepository>();
            builder.Services.AddScoped<IRatingService, RatingService>();
         
            builder.Services.AddScoped<ITourGuideRatingRepository, TourGuideRatingRepository>();

            builder.Services.AddScoped<IBookingProcessingService, BookingProcessingService>();
            builder.Services.AddHostedService<BookingStatusUpdaterService>();

            builder.Services.AddScoped<IDashboardService, DashboardService>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddSignalR();

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapIdentityApi<IdentityUser>();
            app.UseHttpsRedirection();
			      app.UseStaticFiles();
            app.UseSession();
            //app.MapHub<NotificationHub>("/notificationHub");
            app.MapHub<RatingHub>("/ratingHub");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Gọi SeedData.Initialize trong Program.cs
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    await SeedData.Initialize(services);
                    logger.LogInformation("Seed admin data success: ");
                }
                catch (Exception ex)
                {
                    
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            app.Run();
        }

    }
}

