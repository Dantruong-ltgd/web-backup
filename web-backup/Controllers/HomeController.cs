using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_backup.Models;
using web_backup.Data;
using Microsoft.AspNetCore.Authorization; // 👈 Thêm namespace để sử dụng Authorize
using System;
using System.Linq;
using System.Threading.Tasks;

namespace web_backup.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TRANG CHỦ: Hiển thị phòng nổi bật (thả tim) VÀ phòng từ Chủ trọ VIP
        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();

            // Lấy danh sách phòng nổi bật thông thường (thả tim và chưa thuê)
            var favoritedRooms = await _context.Rooms
                .Include(r => r.Category)
                .Where(r => r.IsFeatured && !r.IsRented)
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            // 💥 BỔ SUNG: Lấy danh sách phòng thuộc về các Chủ trọ VIP (còn hạn) và chưa thuê
            var vipRooms = await _context.Rooms
                .Include(r => r.User)
                .Include(r => r.Category)
                .Where(r => !r.IsRented && r.User != null && r.User.IsVip && r.User.VipExpiryDate > DateTime.Now)
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            ViewBag.VipRooms = vipRooms;

            return View(favoritedRooms);
        }

        // 📌 BỔ SUNG: XỬ LÝ THẢ TIM YÊU THÍCH (TỰ ĐỘNG ĐƯA LÊN DÒNG PHÒNG NỔI BẬT TRANG CHỦ)
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return Json(new { success = false, message = "Không tìm thấy thông tin phòng!" });

            // Đảo trạng thái Yêu thích / Nổi bật
            room.IsFeatured = !room.IsFeatured;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isFeatured = room.IsFeatured });
        }

        // 2. TRANG PHÒNG TRỌ: Chỉ hiện các phòng CHƯA THUÊ/MUA
        public async Task<IActionResult> BoardingHouse()
        {
            ViewData["Title"] = "Phòng Trọ";
            ViewData["Heading"] = "DANH SÁCH PHÒNG TRỌ";

            var rooms = await _context.Rooms
                .Include(r => r.Category)
                .Where(r => !r.IsRented && r.Category != null && (r.Category.Name.Contains("Phòng trọ") || r.Category.Name.Contains("Trọ")))
                .ToListAsync();

            return View("CategoryRooms", rooms);
        }

        // 3. TRANG CHUNG CƯ: Chỉ hiện các phòng CHƯA THUÊ/MUA
        public async Task<IActionResult> Apartment()
        {
            ViewData["Title"] = "Chung Cư";
            ViewData["Heading"] = "DANH SÁCH CHUNG CƯ CĂN HỘ";

            var rooms = await _context.Rooms
                .Include(r => r.Category)
                .Where(r => !r.IsRented && r.Category != null && (r.Category.Name.Contains("Chung cư") || r.Category.Name.Contains("Căn hộ")))
                .ToListAsync();

            return View("CategoryRooms", rooms);
        }

        // 4. TRANG NHÀ NGUYÊN CĂN: Chỉ hiện các phòng CHƯA THUÊ/MUA
        public async Task<IActionResult> House()
        {
            ViewData["Title"] = "Nhà Nguyên Căn";
            ViewData["Heading"] = "DANH SÁCH NHÀ NGUYÊN CĂN";

            var rooms = await _context.Rooms
                .Include(r => r.Category)
                .Where(r => !r.IsRented && r.Category != null && (r.Category.Name.Contains("Nhà nguyên căn") || r.Category.Name.Contains("Nguyên căn")))
                .ToListAsync();

            return View("CategoryRooms", rooms);
        }

        // 5. TRANG Ở GHÉP: Chỉ hiện các phòng CHƯA THUÊ/MUA
        public async Task<IActionResult> SharedRoom()
        {
            ViewData["Title"] = "Ở Ghép";
            ViewData["Heading"] = "TÌM PHÒNG Ở GHÉP";

            var rooms = await _context.Rooms
                .Include(r => r.Category)
                .Where(r => !r.IsRented && r.Category != null && (r.Category.Name.Contains("Ở ghép") || r.Category.Name.Contains("Ghép")))
                .ToListAsync();

            return View("CategoryRooms", rooms);
        }

        // 6. XỬ LÝ TÌM KIẾM: Loại bỏ phòng ĐÃ THUÊ/MUA khỏi kết quả tìm kiếm
        public async Task<IActionResult> Search(string? keyword, int? categoryId, string? maxPrice)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();

            var query = _context.Rooms
                .Include(r => r.Category)
                .Where(r => !r.IsRented)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(r => r.Title.Contains(keyword) || r.Address.Contains(keyword) || r.District.Contains(keyword));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(r => r.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(maxPrice))
            {
                var priceParts = maxPrice.Split('-');
                if (priceParts.Length == 2 && decimal.TryParse(priceParts[0], out decimal min) && decimal.TryParse(priceParts[1], out decimal max))
                {
                    query = query.Where(r => r.Price >= min && r.Price <= max);
                }
            }

            var results = await query.ToListAsync();
            return View("Index", results);
        }

        // GET: /Home/Details/5 (Không cho truy cập trang chi tiết phòng đã thuê/mua)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsRented);

            if (room == null) return NotFound();

            return View(room);
        }

        // --- CÁC TRANG HỖ TRỢ KHÁCH HÀNG ---
        public IActionResult HelpCenter()
        {
            ViewData["Title"] = "Trung Tâm Trợ Giúp";
            return View();
        }

        public IActionResult PostingRules()
        {
            ViewData["Title"] = "Quy Định Đăng Tin";
            return View();
        }

        // 💥 GIỚI HẠN QUYỀN: Chỉ tài khoản Admin và Chủ trọ mới được truy cập Bảng giá
        [Authorize(Roles = "Admin,ChuTro")]
        public IActionResult Pricing()
        {
            ViewData["Title"] = "Bảng Giá Dịch Vụ";
            return View();
        }

        // --- CÁC TRANG ĐIỀU KHOẢN ---
        public IActionResult Privacy()
        {
            ViewData["Title"] = "Chính Sách Bảo Mật";
            return View();
        }

        public IActionResult DisputeResolution()
        {
            ViewData["Title"] = "Giải Quyết Tranh Chấp";
            return View();
        }

        public IActionResult TermsOfService()
        {
            ViewData["Title"] = "Điều Khoản Sử Dụng";
            return View();
        }
    }
}