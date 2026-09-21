using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using web_backup.Models;

namespace web_backup.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Amenity> Amenities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Bắt buộc cho Identity

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Phòng trọ" },
                new Category { Id = 2, Name = "Chung cư mini" }
            );

            // Seed Rooms (Đã bổ sung District)
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Title = "Phòng trọ Q1 full nội thất", Price = 2800000, Area = 16.6, Address = "123 Nguyễn Trãi", District = "Quận 1", ImageUrl = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267", IsOwner = true, HasMezzanine = true, CategoryId = 1 },
                new Room { Id = 2, Title = "Chung cư mini Bình Thạnh", Price = 3200000, Area = 25.0, Address = "45 Lê Quang Định", District = "Bình Thạnh", ImageUrl = "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688", IsOwner = true, HasMezzanine = true, CategoryId = 2 },
                new Room { Id = 3, Title = "Phòng trọ gác lửng Q10", Price = 3700000, Area = 30.0, Address = "789 Lý Thường Kiệt", District = "Quận 10", ImageUrl = "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2", IsOwner = true, HasMezzanine = true, CategoryId = 1 }
            );
        }
    }
}