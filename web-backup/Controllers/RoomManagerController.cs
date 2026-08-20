using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web_backup.Data;
using web_backup.Models;

namespace web_backup.Controllers
{
    [Authorize(Roles = "Admin,ChuTro")]
    public class RoomManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoomManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Danh sách phòng trọ
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var query = _context.Rooms.Include(r => r.Category).AsQueryable();

            // Nếu là Chủ trọ -> Chỉ hiển thị danh sách phòng thuộc Quận của mình
            if (User.IsInRole("ChuTro"))
            {
                query = query.Where(r => r.District == currentUser!.District);
            }

            return View(await query.ToListAsync());
        }

        // 2. Trang chỉnh sửa thông tin phòng (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            // Kiểm tra phân quyền theo Quận nếu là Chủ trọ
            if (User.IsInRole("ChuTro") && !string.Equals(room.District, currentUser?.District, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", room.CategoryId);
            return View(room);
        }

        // 3. Xử lý lưu thông tin & hình ảnh chỉnh sửa (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.Id) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            // Kiểm tra phân quyền
            if (User.IsInRole("ChuTro") && !string.Equals(room.District, currentUser?.District, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Rooms.Any(e => e.Id == room.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", room.CategoryId);
            return View(room);
        }

        // 4. Trang hiển thị Form thêm phòng mới (GET)
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Gửi thông tin Quận của Chủ trọ sang View để tự động điền nếu là Chủ trọ
            ViewBag.UserDistrict = currentUser?.District;
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");

            return View();
        }

        // 5. Xử lý lưu phòng mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Nếu là Chủ trọ -> Tự động gán Quận theo tài khoản của Chủ trọ
            if (User.IsInRole("ChuTro") && !string.IsNullOrEmpty(currentUser?.District))
            {
                room.District = currentUser.District;
            }

            // 💥 GÁN NGƯỜI TẠO VÀ TỰ ĐỘNG ĐƯA LÊN TIN VIP NẾU TÀI KHOẢN CÒN HẠN VIP
            if (currentUser != null)
            {
                room.UserId = currentUser.Id; // Gán UserId cho phòng

                if (currentUser.IsVip && currentUser.VipExpiryDate.HasValue && currentUser.VipExpiryDate.Value > DateTime.Now)
                {
                    room.IsFeatured = true; // Tự động bật Tin VIP / Nổi bật
                }
                else
                {
                    room.IsFeatured = false; // Bài đăng thường
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", room.CategoryId);
            return View(room);
        }

        // 6. Xử lý bật/tắt trạng thái Đã thuê / Mua qua AJAX (POST)
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            // Kiểm tra phân quyền theo Quận nếu là Chủ trọ
            if (User.IsInRole("ChuTro") && !string.Equals(room.District, currentUser?.District, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            // Đảo trạng thái Đã thuê/Mua
            room.IsRented = !room.IsRented;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, isRented = room.IsRented });
        }

        // 7. Xử lý xóa phòng qua AJAX (POST)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            // Kiểm tra phân quyền theo Quận nếu là Chủ trọ
            if (User.IsInRole("ChuTro") && !string.Equals(room.District, currentUser?.District, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            try
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "Không thể xóa phòng này vì đã có dữ liệu đặt phòng/hóa đơn liên quan!" });
            }
        }
    }
}