using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web_backup.Models;

namespace web_backup.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // ==========================================
            // 1. TẠO CÁC QUYỀN (ROLES)
            // ==========================================
            string[] roles = { "Admin", "ChuTro", "NguoiXem" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ==========================================
            // 2. TẠO TÀI KHOẢN MẪU
            // ==========================================

            // 2.1 Admin (Toàn quyền)
            var adminEmail = "admin@ptsg.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Quản Trị Viên",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // 2.2 Chủ Trọ (Quản lý phòng ở Bình Thạnh)
            var chuTroEmail = "chutro_binhthanh@ptsg.com";
            if (await userManager.FindByEmailAsync(chuTroEmail) == null)
            {
                var chuTro = new ApplicationUser
                {
                    UserName = chuTroEmail,
                    Email = chuTroEmail,
                    FullName = "Chủ Trọ Bình Thạnh",
                    District = "Bình Thạnh",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(chuTro, "ChuTro@123");
                await userManager.AddToRoleAsync(chuTro, "ChuTro");
            }

            // 2.3 Người Xem
            var userEmail = "user@ptsg.com";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FullName = "Khách Hàng Xem Phòng",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, "NguoiXem");
            }

            // ==========================================
            // 3. TẠO CÁC LOẠI PHÒNG (CATEGORIES)
            // ==========================================
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Phòng trọ" },
                    new Category { Name = "Chung cư" },
                    new Category { Name = "Nhà nguyên căn" },
                    new Category { Name = "Ở ghép" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // ==========================================
            // 4. TẠO DỮ LIỆU PHÒNG MẪU (TEST DATA)
            // ==========================================
            if (!context.Rooms.Any())
            {
                var catPhongTro = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Phòng trọ");
                var catChungCu = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Chung cư");
                var catNhaNC = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Nhà nguyên căn");
                var catOGhep = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Ở ghép");

                var sampleRooms = new List<Room>
                {
                    new Room
                    {
                        Title = "Phòng trọ gác lửng đầy đủ tiện nghi",
                        Price = 3500000,
                        Area = 25,
                        District = "Bình Thạnh",
                        Address = "123 Điện Biên Phủ, P.25",
                        ImageUrl = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?q=80&w=800",
                        IsOwner = true,
                        HasMezzanine = true,
                        CategoryId = catPhongTro?.Id ?? 1
                    },
                    new Room
                    {
                        Title = "Căn hộ chung cư 2 phòng ngủ cao cấp",
                        Price = 8500000,
                        Area = 65,
                        District = "Quận 1",
                        Address = "45 Nguyễn Thị Minh Khai, P. Bến Nghé",
                        ImageUrl = "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?q=80&w=800",
                        IsOwner = true,
                        HasMezzanine = false,
                        CategoryId = catChungCu?.Id ?? 2
                    },
                    new Room
                    {
                        Title = "Nhà nguyên căn 1 trệt 1 lầu hẻm xe hơi",
                        Price = 12000000,
                        Area = 80,
                        District = "Bình Thạnh",
                        Address = "78 Xô Viết Nghệ Tĩnh, P.21",
                        ImageUrl = "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?q=80&w=800",
                        IsOwner = true,
                        HasMezzanine = false,
                        CategoryId = catNhaNC?.Id ?? 3
                    },
                    new Room
                    {
                        Title = "Tìm nam ở ghép căn hộ chung cư Vinhomes",
                        Price = 1800000,
                        Area = 20,
                        District = "Bình Thạnh",
                        Address = "208 Nguyễn Hữu Cảnh, P.22",
                        ImageUrl = "https://images.unsplash.com/photo-1555854877-bab0e564b8d5?q=80&w=800",
                        IsOwner = false,
                        HasMezzanine = false,
                        CategoryId = catOGhep?.Id ?? 4
                    }
                };

                await context.Rooms.AddRangeAsync(sampleRooms);
                await context.SaveChangesAsync();
            }
        }
    }
}