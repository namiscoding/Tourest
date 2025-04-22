using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;

namespace Tourest.Data
{
	public class ApplicationDbContext : DbContext
    {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
		{
		}

		// DbSets
		public DbSet<User> Users { get; set; } = null!; // Use `= null!` to suppress nullable warnings if needed
		public DbSet<Account> Accounts { get; set; } = null!;
		public DbSet<Tourest.Data.Entities.TourGuide> TourGuides { get; set; } = null!;
		public DbSet<Category> Categories { get; set; } = null!;
		public DbSet<Tour> Tours { get; set; } = null!;
		public DbSet<TourCategory> TourCategories { get; set; } = null!;
		public DbSet<ItineraryDay> ItineraryDays { get; set; } = null!;
		public DbSet<Booking> Bookings { get; set; } = null!;
		public DbSet<Payment> Payments { get; set; } = null!;
		public DbSet<TourGroup> TourGroups { get; set; } = null!;
		public DbSet<TourGuideAssignment> TourGuideAssignments { get; set; } = null!;
		public DbSet<Rating> Ratings { get; set; } = null!;
		public DbSet<TourRating> TourRatings { get; set; } = null!;
		public DbSet<TourGuideRating> TourGuideRatings { get; set; } = null!;
		public DbSet<TourAuditLog> TourAuditLogs { get; set; } = null!;
		public DbSet<Notification> Notifications { get; set; } = null!;
		public DbSet<SupportRequest> SupportRequests { get; set; } = null!;


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// --- Fluent API Configurations ---

			// User Configuration
			modelBuilder.Entity<User>(entity =>
			{
				entity.ToTable("Users");
				entity.HasKey(e => e.UserID);
				entity.Property(e => e.UserID).ValueGeneratedOnAdd(); // Identity

				entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
				entity.HasIndex(e => e.Email).IsUnique(); // Unique constraint for Email

				entity.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired(false); // NULL
				entity.Property(e => e.Address).HasMaxLength(500).IsRequired(false); // NULL
				entity.Property(e => e.ProfilePictureUrl).HasMaxLength(1024).IsRequired(false); // NULL

				entity.Property(e => e.RegistrationDate).IsRequired().HasDefaultValueSql("SYSDATETIME()");
				entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

				// Relationships configured via the dependent side usually
			});

			// Account Configuration
			modelBuilder.Entity<Account>(entity =>
			{
				entity.ToTable("Accounts");
				entity.HasKey(e => e.AccountID);
				entity.Property(e => e.AccountID).ValueGeneratedOnAdd();

				entity.Property(e => e.UserID).IsRequired();
				entity.HasIndex(e => e.UserID).IsUnique(); // For 1-1 relationship

				entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
				entity.HasIndex(e => e.Username).IsUnique();

				entity.Property(e => e.PasswordHash).IsRequired(); // Max length implied by NVARCHAR(MAX)
				entity.Property(e => e.Role).IsRequired().HasMaxLength(50);

				entity.Property(e => e.PasswordResetToken).HasMaxLength(255).IsRequired(false);
				entity.Property(e => e.ResetTokenExpiration).IsRequired(false);

				// Define 1-1 relationship explicitly (optional if conventions work)
				entity.HasOne(a => a.User)
					  .WithOne(u => u.Account)
					  .HasForeignKey<Account>(a => a.UserID)
					  .OnDelete(DeleteBehavior.Cascade); // Delete Account if User is deleted
			});

			// TourGuide Configuration (TPT Inheritance)
			modelBuilder.Entity<Tourest.Data.Entities.TourGuide>(entity =>
			{
				entity.ToTable("TourGuides"); // Explicitly map to its own table
				entity.HasKey(e => e.TourGuideUserID); // PK is the FK to User

				entity.Property(e => e.ExperienceLevel).HasMaxLength(50).IsRequired(false);
				entity.Property(e => e.LanguagesSpoken).HasMaxLength(500).IsRequired(false);
				entity.Property(e => e.Specializations).HasMaxLength(500).IsRequired(false);
				entity.Property(e => e.MaxGroupSizeCapacity).IsRequired(false);
				entity.Property(e => e.AverageRating).HasColumnType("decimal(3, 2)").IsRequired(false);

				// Define 1-1 relationship (Subtype to Base)
				entity.HasOne(tg => tg.User)
					  .WithOne(u => u.TourGuide) // Assuming User has 'TourGuide' navigation property
					  .HasForeignKey<Tourest.Data.Entities.TourGuide>(tg => tg.TourGuideUserID)
					  .OnDelete(DeleteBehavior.Cascade); // Delete TourGuide profile if User is deleted
			});

			// Category Configuration
			modelBuilder.Entity<Category>(entity =>
			{
				entity.ToTable("Categories");
				entity.HasKey(e => e.CategoryID);
				entity.Property(e => e.CategoryID).ValueGeneratedOnAdd();
				entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
				entity.HasIndex(e => e.Name).IsUnique();
				entity.Property(e => e.Description).HasMaxLength(500).IsRequired(false);
			});

			// Tour Configuration
			modelBuilder.Entity<Tour>(entity =>
			{
				entity.ToTable("Tours");
				entity.HasKey(e => e.TourID);
				entity.Property(e => e.TourID).ValueGeneratedOnAdd();

				entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Destination).IsRequired().HasMaxLength(255);
				entity.Property(e => e.Description).IsRequired(false); // Default for NVARCHAR(MAX)
				entity.Property(e => e.DurationDays).IsRequired();
				entity.Property(e => e.DurationNights).IsRequired();
				entity.Property(e => e.AdultPrice).IsRequired();
				entity.Property(e => e.ChildPrice).IsRequired();
				entity.Property(e => e.MinGroupSize).IsRequired(false);
				entity.Property(e => e.MaxGroupSize).IsRequired(false);
				entity.Property(e => e.DeparturePoints).HasMaxLength(500).IsRequired(false);
				entity.Property(e => e.IncludedServices).IsRequired(false);
				entity.Property(e => e.ExcludedServices).IsRequired(false);
				entity.Property(e => e.ImageUrls).IsRequired(false);
				entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
				entity.Property(e => e.AverageRating).HasColumnType("decimal(3, 2)").IsRequired(false);
				entity.Property(e => e.IsCancellable).IsRequired().HasDefaultValue(false);
				entity.Property(e => e.CancellationPolicyDescription).IsRequired(false);
			});

			// TourCategory Configuration (Many-to-Many Join Table)
			modelBuilder.Entity<TourCategory>(entity =>
			{
				entity.ToTable("TourCategories");
				entity.HasKey(tc => new { tc.TourID, tc.CategoryID }); // Composite Key

				entity.HasOne(tc => tc.Tour)
					  .WithMany(t => t.TourCategories)
					  .HasForeignKey(tc => tc.TourID)
					  .OnDelete(DeleteBehavior.Cascade); // Delete junction record if Tour deleted

				entity.HasOne(tc => tc.Category)
					  .WithMany(c => c.TourCategories)
					  .HasForeignKey(tc => tc.CategoryID)
					  .OnDelete(DeleteBehavior.Cascade); // Delete junction record if Category deleted
			});

			// ItineraryDay Configuration
			modelBuilder.Entity<ItineraryDay>(entity =>
			{
				entity.ToTable("ItineraryDays");
				entity.HasKey(e => e.ItineraryDayID);
				entity.Property(e => e.ItineraryDayID).ValueGeneratedOnAdd();

				entity.Property(e => e.TourID).IsRequired();
				entity.Property(e => e.DayNumber).IsRequired();
				entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Description).IsRequired(false);
				entity.Property(e => e.Order).IsRequired().HasDefaultValue(0);

				entity.HasOne(id => id.Tour)
					  .WithMany(t => t.ItineraryDays)
					  .HasForeignKey(id => id.TourID)
					  .OnDelete(DeleteBehavior.Cascade); // Delete days if Tour deleted
			});

			// Booking Configuration
			modelBuilder.Entity<Booking>(entity =>
			{
				entity.ToTable("Bookings");
				entity.HasKey(e => e.BookingID);
				entity.Property(e => e.BookingID).ValueGeneratedOnAdd();

				entity.Property(e => e.BookingDate).IsRequired().HasDefaultValueSql("SYSDATETIME()");
				entity.Property(e => e.DepartureDate).IsRequired().HasColumnType("DATE"); // Specify Date type
				entity.Property(e => e.NumberOfAdults).IsRequired();
				entity.Property(e => e.NumberOfChildren).IsRequired().HasDefaultValue(0);
				entity.Property(e => e.TotalPrice).IsRequired();
				entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
				entity.Property(e => e.PickupPoint).HasMaxLength(200).IsRequired(false);
				entity.Property(e => e.CustomerID).IsRequired();
				entity.Property(e => e.TourID).IsRequired();
				entity.Property(e => e.TourGroupID).IsRequired(false);
				entity.Property(e => e.PaymentID).IsRequired(false);
				entity.Property(e => e.CancellationDate).IsRequired(false);
				entity.Property(e => e.RefundAmount).IsRequired(false);

				entity.HasOne(b => b.Customer)
					  .WithMany(u => u.Bookings)
					  .HasForeignKey(b => b.CustomerID)
					  .OnDelete(DeleteBehavior.Restrict); // Prevent deleting user with bookings

				entity.HasOne(b => b.Tour)
					  .WithMany(t => t.Bookings)
					  .HasForeignKey(b => b.TourID)
					  .OnDelete(DeleteBehavior.Restrict); // Prevent deleting tour with bookings

				entity.HasOne(b => b.TourGroup)
					  .WithMany(tg => tg.Bookings)
					  .HasForeignKey(b => b.TourGroupID)
					  .OnDelete(DeleteBehavior.SetNull); // Set null if group deleted

				// Configured from Payment side for 1:1/1:0..1
				// entity.HasOne(b => b.Payment)
				//       .WithOne(p => p.Booking)
				//       .HasForeignKey<Booking>(b => b.PaymentID) // Careful with 1:1 FK placement
				//       .OnDelete(DeleteBehavior.SetNull);
			});

			// Payment Configuration
			modelBuilder.Entity<Payment>(entity =>
			{
				entity.ToTable("Payments");
				entity.HasKey(e => e.PaymentID);
				entity.Property(e => e.PaymentID).ValueGeneratedOnAdd();

				entity.Property(e => e.BookingID).IsRequired();
				entity.HasIndex(e => e.BookingID).IsUnique(); // Ensure 1 payment per booking

				entity.Property(e => e.Amount).IsRequired();
				entity.Property(e => e.PaymentDate).IsRequired();
				entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
				entity.Property(e => e.TransactionID).HasMaxLength(255).IsRequired(false);
				entity.Property(e => e.Status).IsRequired().HasMaxLength(50);

				// Define 1:1 relationship with Booking
				entity.HasOne(p => p.Booking)
					  .WithOne(b => b.Payment) // Assumes Booking has 'Payment' nav prop
					  .HasForeignKey<Payment>(p => p.BookingID) // FK is on Payment side
					  .OnDelete(DeleteBehavior.Cascade); // Delete payment if booking deleted
			});

			// TourGroup Configuration
			modelBuilder.Entity<TourGroup>(entity =>
			{
				entity.ToTable("TourGroups");
				entity.HasKey(e => e.TourGroupID);
				entity.Property(e => e.TourGroupID).ValueGeneratedOnAdd();

				entity.Property(e => e.TourID).IsRequired();
				entity.Property(e => e.DepartureDate).IsRequired().HasColumnType("DATE");
				entity.Property(e => e.TotalGuests).IsRequired();
				entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
				entity.Property(e => e.CreationDate).IsRequired().HasDefaultValueSql("SYSDATETIME()");
				entity.Property(e => e.AssignedTourGuideID).IsRequired(false);

				entity.HasOne(tg => tg.Tour)
					  .WithMany(t => t.TourGroups)
					  .HasForeignKey(tg => tg.TourID)
					  .OnDelete(DeleteBehavior.Restrict); // Don't delete Tour if Groups exist

				entity.HasOne(tg => tg.AssignedTourGuide)
					  .WithMany() // Assuming User doesn't need collection of Groups assigned
					  .HasForeignKey(tg => tg.AssignedTourGuideID)
					  .OnDelete(DeleteBehavior.SetNull); // Set null if Guide deleted
			});

			// TourGuideAssignment Configuration
			modelBuilder.Entity<TourGuideAssignment>(entity =>
			{
				entity.ToTable("TourGuideAssignments");
				entity.HasKey(e => e.AssignmentID);
				entity.Property(e => e.AssignmentID).ValueGeneratedOnAdd();

				entity.Property(e => e.TourGroupID).IsRequired();
				entity.Property(e => e.TourGuideID).IsRequired();
				entity.Property(e => e.TourManagerID).IsRequired();
				entity.Property(e => e.AssignmentDate).IsRequired().HasDefaultValueSql("SYSDATETIME()");
				entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
				entity.Property(e => e.RejectionReason).HasMaxLength(500).IsRequired(false);
				entity.Property(e => e.ConfirmationDate).IsRequired(false);

				entity.HasOne(a => a.TourGroup)
					  .WithMany(tg => tg.TourGuideAssignments)
					  .HasForeignKey(a => a.TourGroupID)
					  .OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(a => a.TourGuide)
					  .WithMany(u => u.TourGuideAssignments) // User as Guide
					  .HasForeignKey(a => a.TourGuideID)
					  .OnDelete(DeleteBehavior.Restrict); // Don't delete user if assignments exist

				entity.HasOne(a => a.TourManager)
					  .WithMany(u => u.TourManagerAssignments) // User as Manager
					  .HasForeignKey(a => a.TourManagerID)
					  .OnDelete(DeleteBehavior.Restrict); // Don't delete user if assignments exist
			});

			// Rating Configuration (Base)
			modelBuilder.Entity<Rating>(entity =>
			{
				entity.ToTable("Ratings");
				entity.HasKey(e => e.RatingID);
				entity.Property(e => e.RatingID).ValueGeneratedOnAdd();

				entity.Property(e => e.CustomerID).IsRequired();
				entity.Property(e => e.RatingValue).IsRequired().HasColumnType("decimal(3, 2)");
				entity.Property(e => e.Comment).IsRequired(false);
				entity.Property(e => e.RatingDate).IsRequired().HasDefaultValueSql("SYSDATETIME()");
				entity.Property(e => e.RatingType).IsRequired().HasMaxLength(20); // Used implicitly by TPT

				entity.HasOne(r => r.Customer)
					  .WithMany(u => u.Ratings)
					  .HasForeignKey(r => r.CustomerID)
					  .OnDelete(DeleteBehavior.Restrict); // Delete ratings if customer deleted? Or Restrict?
			});

			// TourRating Configuration (TPT mapping)
			modelBuilder.Entity<TourRating>(entity =>
			{
				entity.ToTable("TourRatings"); // Map to its own table
				entity.HasKey(e => e.RatingID); // PK is FK to Rating base table

				entity.Property(e => e.TourID).IsRequired();

				// Define 1:1 relationship to base Rating table
				entity.HasOne(tr => tr.Rating)
					  .WithOne(r => r.TourRating) // Assumes Rating has TourRating nav prop
					  .HasForeignKey<TourRating>(tr => tr.RatingID)
					  .OnDelete(DeleteBehavior.Cascade); // Delete this if base rating deleted

				entity.HasOne(tr => tr.Tour)
					  .WithMany(t => t.TourRatings)
					  .HasForeignKey(tr => tr.TourID)
					  .OnDelete(DeleteBehavior.Cascade); // Delete rating if tour deleted
			});

			// TourGuideRating Configuration (TPT mapping)
			modelBuilder.Entity<TourGuideRating>(entity =>
			{
				entity.ToTable("TourGuideRatings"); // Map to its own table
				entity.HasKey(e => e.RatingID); // PK is FK to Rating base table

				entity.Property(e => e.TourGuideID).IsRequired();
				entity.Property(e => e.TourGroupID).IsRequired();

				// Define 1:1 relationship to base Rating table
				entity.HasOne(tgr => tgr.Rating)
					  .WithOne(r => r.TourGuideRating) // Assumes Rating has TourGuideRating nav prop
					  .HasForeignKey<TourGuideRating>(tgr => tgr.RatingID)
					  .OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(tgr => tgr.TourGuide)
					  .WithMany(u => u.TourGuideRatingsReceived)
					  .HasForeignKey(tgr => tgr.TourGuideID)
					  .OnDelete(DeleteBehavior.Restrict); // Delete rating if guide deleted? Or Restrict?

				entity.HasOne(tgr => tgr.TourGroup)
					  .WithMany(tg => tg.TourGuideRatings)
					  .HasForeignKey(tgr => tgr.TourGroupID)
					  .OnDelete(DeleteBehavior.Cascade); // Delete rating if group deleted
			});


			// TourAuditLog Configuration
			modelBuilder.Entity<TourAuditLog>(entity =>
			{
				entity.ToTable("TourAuditLogs");
				entity.HasKey(e => e.AuditLogID);
				entity.Property(e => e.AuditLogID).ValueGeneratedOnAdd(); // BIGINT Identity

				entity.Property(e => e.TourID).IsRequired();
				entity.Property(e => e.ActionType).IsRequired().HasMaxLength(10);
				entity.Property(e => e.PerformedByUserID).IsRequired();
				entity.Property(e => e.Timestamp).IsRequired().HasDefaultValueSql("SYSDATETIME()");
				entity.Property(e => e.OldValues).IsRequired(false);
				entity.Property(e => e.NewValues).IsRequired(false);

				entity.HasOne(l => l.Tour)
					  .WithMany(t => t.AuditLogs)
					  .HasForeignKey(l => l.TourID)
					  .OnDelete(DeleteBehavior.Cascade); // Or Restrict/SetNull depending on policy

				entity.HasOne(l => l.PerformedBy)
					  .WithMany(u => u.TourAuditLogsPerformed)
					  .HasForeignKey(l => l.PerformedByUserID)
					  .OnDelete(DeleteBehavior.Restrict); // Don't delete user easily if logs exist
			});

			// Notification Configuration
			modelBuilder.Entity<Notification>(entity =>
			{
				entity.ToTable("Notifications");
				entity.HasKey(e => e.NotificationID);
				entity.Property(e => e.NotificationID).ValueGeneratedOnAdd();

				entity.Property(e => e.RecipientUserID).IsRequired();
				entity.Property(e => e.SenderUserID).IsRequired(false); // Nullable FK
				entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Title).HasMaxLength(255).IsRequired(false);
				entity.Property(e => e.Content).IsRequired();
				entity.Property(e => e.RelatedEntityID).HasMaxLength(50).IsRequired(false);
				entity.Property(e => e.RelatedEntityType).HasMaxLength(50).IsRequired(false);
				entity.Property(e => e.Timestamp).IsRequired().HasDefaultValueSql("SYSDATETIME()");
				entity.Property(e => e.IsRead).IsRequired().HasDefaultValue(false);
				entity.Property(e => e.ActionUrl).HasMaxLength(1024).IsRequired(false);

				entity.HasOne(n => n.RecipientUser)
					  .WithMany(u => u.NotificationsReceived)
					  .HasForeignKey(n => n.RecipientUserID)
					  .OnDelete(DeleteBehavior.NoAction); // Delete notification if recipient deleted

				entity.HasOne(n => n.SenderUser)
					  .WithMany(u => u.NotificationsSent)
					  .HasForeignKey(n => n.SenderUserID)
					  .OnDelete(DeleteBehavior.SetNull); // Set sender null if sender deleted
			});

			// SupportRequest Configuration
			modelBuilder.Entity<SupportRequest>(entity =>
			{
				entity.ToTable("SupportRequests");
				entity.HasKey(e => e.RequestID);
				entity.Property(e => e.RequestID).ValueGeneratedOnAdd();

				entity.Property(e => e.CustomerID).IsRequired();
				entity.Property(e => e.Subject).IsRequired().HasMaxLength(255);
				entity.Property(e => e.Message).IsRequired();
				entity.Property(e => e.SubmissionDate).IsRequired().HasDefaultValueSql("SYSDATETIME()");
				entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
				entity.Property(e => e.HandlerUserID).IsRequired(false); // Nullable FK
				entity.Property(e => e.ResolutionNotes).IsRequired(false);

				entity.HasOne(sr => sr.Customer)
					  .WithMany(u => u.SubmittedSupportRequests)
					  .HasForeignKey(sr => sr.CustomerID)
					  .OnDelete(DeleteBehavior.Restrict); // Prevent deleting customer with open requests?

				entity.HasOne(sr => sr.HandlerUser)
					  .WithMany(u => u.HandledSupportRequests)
					  .HasForeignKey(sr => sr.HandlerUserID)
					  .OnDelete(DeleteBehavior.SetNull); // Set handler null if handler deleted
			});
		}
	}
}
