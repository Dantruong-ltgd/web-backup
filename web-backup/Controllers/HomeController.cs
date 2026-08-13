using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_backup.Models;
using web_backup.Data;

namespace web_backup.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TRANG CHỦ: Chỉ hiển thị các căn nổi bật (gồm tất cả các loại phòng)
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách Loại phòng nạp vào Dropdown tìm kiếm
            ViewBag.Categories = await _context.Categories.ToListAsync();

            // Lấy tất cả phòng (hoặc lọc theo tiêu chí nổi bật như phòng mới nhất/chính chủ)
            var featuredRooms = await _context.Rooms
                .Include(r => r.Category)
                .OrderByDescending(r => r.Id) // Bạn có thể thêm .Where(r => r.IsFeatured) nếu có thuộc tính IsFeatured
                .Take(6) // Lấy 6 căn nổi bật nhất
                .ToListAsync();

            return View(featuredRooms);
        }

        // 2. TRANG PHÒNG TRỌ
        public async Task<IActionResult> BoardingHouse()
        {
            ViewData["Title"] = "Phòng Trọ";
            ViewData["Heading"] = "DANH SÁCH PHÒNG TRỌ";

            var rooms = await _context.Rooms
                .Include(r => r.Category)
                .Where(r => r.Category != null && (r.Category.Name.Contains("Phòng trọ") || r.Category.Name.Contains("Trọ")))
                .ToListAsync();

            return View("CategoryRooms", rooms);
        }

        // 3. TRANG CHUNG CƯ
        public async Task<IActionResult> Apartment()
        {
            ViewData["Title"] = "Chung Cư";
            ViewData["Heading"] = "DANH SÁCH CHUNG CƯ CĂN HỘ";

            var rooms = await _context.Rooms
                .Include(r => r.Category)
                .Where(r => r.Category != null && (r.Category.Name.Contains("Chung cư") || r.Category.Name.Contains("Căn hộ")))
                .ToListAsync();

            return View("CategoryRooms", rooms);
        }

        // 4. TRANG NHÀ NGUYÊN CĂN
        public async Task<IActionResult> House()
        {
            ViewData["Title"] = "Nhà Nguyên Căn";
            ViewData["Heading"] = "DANH SÁCH NHÀ NGUYÊN CĂN";

            var rooms = await _context.Rooms
                .Include(r => r.Category)
                .Where(r => r.Category != null && (r.Category.Name.Contains("Nhà nguyên căn") || r.Category.Name.Contains("Nguyên căn")))
                .ToListAsync();

            return View("CategoryRooms", rooms);
        }

        // 5. TRANG Ở GHÉP
        public async Task<IActionResult> SharedRoom()
        {
            ViewData["Title"] = "Ở Ghép";
            ViewData["Heading"] = "TÌM PHÒNG Ở GHÉP";

            var rooms = await _context.Rooms
                .Include(r => r.Category)
                .Where(r => r.Category != null && (r.Category.Name.Contains("Ở ghép") || r.Category.Name.Contains("Ghép")))
                .ToListAsync();

            return View("CategoryRooms", rooms);
        }

        // 6. XỬ LÝ TÌM KIẾM
        public async Task<IActionResult> Search(string? keyword, int? categoryId, string? maxPrice)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();

            var query = _context.Rooms.Include(r => r.Category).AsQueryable();

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
        // GET: /Home/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

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