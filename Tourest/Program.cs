    using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tourest.Data;
using Tourest.Data.Repositories;
using Tourest.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Tourest
{
	public class Program
	{
		public static void Main(string[] args)
		{

			var builder = WebApplication.CreateBuilder(args);

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


            builder.Services.AddAuthentication()
    .AddCookie(options => {
        options.Events.OnValidatePrincipal = context => {
            var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            context.Principal.AddIdentity(new ClaimsIdentity(new[] { new Claim("userId", userId) }));
            return Task.CompletedTask;
        };
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
            // Add Repository
            builder.Services.AddScoped<ITourRepository, TourRepository>();

            builder.Services.AddScoped<ITourService, TourService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); 
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ITourGuideService, TourGuideService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();


            // Add services to the container.
            builder.Services.AddScoped<ITourService, TourService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddControllersWithViews();
            
            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapIdentityApi<IdentityUser>();
            app.UseHttpsRedirection();
			app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
         
            //app.MapHub<NotificationHub>("/notificationHub");

            app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Tours}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
