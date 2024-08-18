using Microsoft.EntityFrameworkCore;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Recovery> Recoveries { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<FacilityImage> Images { get; set; }
        public DbSet<Favourite> Favourites { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>().ToTable("Notification");
            modelBuilder.Entity<Recovery>().ToTable("Recovery");
            modelBuilder.Entity<Facility>().ToTable("Facility");
            modelBuilder.Entity<Booking>().ToTable("Booking");
            modelBuilder.Entity<Review>().ToTable("Review");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<FacilityImage>().ToTable("Image");
            modelBuilder.Entity<Transaction>().ToTable("Transaction");
            modelBuilder.Entity<Favourite>().ToTable("Favourite");
        }
    }
}
