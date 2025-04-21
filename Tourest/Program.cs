using Microsoft.EntityFrameworkCore;
using Tourest.Data;
using Tourest.Data.Repositories;
using Tourest.Services;
using Tourest.TourGuide.Repositories;
using Tourest.TourGuide.Services;


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

            // Add services to the container.
            builder.Services.AddScoped<ITourRepository, TourRepository>();
            builder.Services.AddScoped<ITourService, TourService>();
            builder.Services.AddScoped<ITourAssignmentService, TourAssignmentService>();
            builder.Services.AddScoped<IAssignedTourRespo, AssignedTourRepository>();
            builder.Services.AddControllersWithViews();
            
            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Tours}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
