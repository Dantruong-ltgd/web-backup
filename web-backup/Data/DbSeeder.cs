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

            // 2.2 Tạo tài khoản Chủ Trọ cho TẤT CẢ các quận/huyện tại TP.HCM
            var districts = new[]
            {
                "Quận 1", "Quận 3", "Quận 4", "Quận 5", "Quận 6", "Quận 7", "Quận 8",
                "Quận 10", "Quận 11", "Quận 12", "Bình Thạnh", "Gò Vấp", "Tân Bình",
                "Tân Phú", "Phú Nhuận", "Bình Tân", "TP. Thủ Đức"
            };

            foreach (var district in districts)
            {
                // Chuẩn hóa tên email (VD: chutro_q1@ptsg.com, chutro_binhthanh@ptsg.com)
                string slug = district.ToLower()
                    .Replace(" ", "")
                    .Replace(".", "")
                    .Replace("quận", "q")
                    .Replace("bìnhthạnh", "binhthanh")
                    .Replace("gòvấp", "govap")
                    .Replace("tânbình", "tanbinh")
                    .Replace("tânphú", "tanphu")
                    .Replace("phúnhuận", "phunhuan")
                    .Replace("bìnhtân", "binhtan")
                    .Replace("tpthủđức", "thuduc");

                string chuTroEmail = $"chutro_{slug}@ptsg.com";

                if (await userManager.FindByEmailAsync(chuTroEmail) == null)
                {
                    // Chỉ riêng Chủ trọ Quận 1 và Quận 7 được cấp gói VIP
                    bool isVip = (district == "Quận 1" || district == "Quận 7");

                    var chuTro = new ApplicationUser
                    {
                        UserName = chuTroEmail,
                        Email = chuTroEmail,
                        FullName = $"Chủ Trọ {district}",
                        District = district,
                        PhoneNumber = "090" + new Random().Next(1000000, 9999999),
                        IsVip = isVip,
                        VipExpiryDate = isVip ? DateTime.Now.AddDays(30) : null,
                        EmailConfirmed = true
                    };

                    var createResult = await userManager.CreateAsync(chuTro, "ChuTro@123");
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(chuTro, "ChuTro");
                    }
                }
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

                // Lấy User ID của các chủ trọ
                var userQ1 = await userManager.FindByEmailAsync("chutro_q1@ptsg.com");
                var userQ7 = await userManager.FindByEmailAsync("chutro_q7@ptsg.com");
                var userBinhThanh = await userManager.FindByEmailAsync("chutro_binhthanh@ptsg.com");

                var sampleRooms = new List<Room>
                {
                    // Bài đăng VIP 1: Thuộc Chủ trọ Quận 1 (Hiển thị nổi bật trên Trang chủ)
                    new Room
                    {
                        Title = "Căn hộ chung cư 2 phòng ngủ cao cấp Quận 1",
                        Price = 8500000,
                        Area = 65,
                        District = "Quận 1",
                        Address = "45 Nguyễn Thị Minh Khai, P. Bến Nghé",
                        ImageUrl = "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?q=80&w=800",
                        IsOwner = true,
                        HasMezzanine = false,
                        IsFeatured = true, // Gói VIP
                        UserId = userQ1?.Id,
                        CategoryId = catChungCu?.Id ?? 2
                    },
                    // Bài đăng VIP 2: Thuộc Chủ trọ Quận 7 (Hiển thị nổi bật trên Trang chủ)
                    new Room
                    {
                        Title = "Phòng trọ cao cấp gác lửng gần Lotte Mart Quận 7",
                        Price = 4500000,
                        Area = 30,
                        District = "Quận 7",
                        Address = "1041 Nguyễn Thị Thập, P. Tân Phong",
                        ImageUrl = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?q=80&w=800",
                        IsOwner = true,
                        HasMezzanine = true,
                        IsFeatured = true, // Gói VIP
                        UserId = userQ7?.Id,
                        CategoryId = catPhongTro?.Id ?? 1
                    },
                    // Bài đăng thường: Thuộc Chủ trọ Bình Thạnh (Không hiển thị ở Trang chủ mới)
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
                        IsFeatured = false,
                        UserId = userBinhThanh?.Id,
                        CategoryId = catPhongTro?.Id ?? 1
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
                        IsFeatured = false,
                        UserId = userBinhThanh?.Id,
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
                        IsFeatured = false,
                        UserId = userBinhThanh?.Id,
                        CategoryId = catOGhep?.Id ?? 4
                    }
                };

                await context.Rooms.AddRangeAsync(sampleRooms);
                await context.SaveChangesAsync();
            }
        }
    }
}