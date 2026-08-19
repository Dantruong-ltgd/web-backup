using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_backup.Data;
using web_backup.Models;

namespace web_backup.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Payment/Checkout?roomId=1&type=Deposit (hoặc Buyout)
        public async Task<IActionResult> Checkout(int roomId, string type = "Deposit")
        {
            var room = await _context.Rooms
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            // Kiểm tra: Chỉ Chung cư hoặc Nhà nguyên căn mới được mua đứt
            if (type == "Buyout")
            {
                bool isBuyoutCategory = room.Category != null &&
                    (room.Category.Name.Contains("Chung cư") || room.Category.Name.Contains("Nhà nguyên căn"));

                if (!room.IsForSale || !isBuyoutCategory)
                {
                    TempData["Error"] = "Sản phẩm này không hỗ trợ thanh toán mua đứt!";
                    return RedirectToAction("Index", "Home");
                }
            }

            // Tính tiền: Cọc 20% hoặc Mua đứt 100%
            decimal paymentAmount = type == "Buyout" ? (room.SalePrice ?? 0) : (room.Price * 0.2m);

            ViewBag.TransactionType = type;
            ViewBag.PaymentAmount = paymentAmount;
            ViewBag.User = user;

            // Ép đường dẫn tuyệt đối tới file View
            return View("~/Views/Payment/Checkout.cshtml", room);
        }

        // POST: /Payment/ProcessPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int roomId, string transactionType, decimal amount, string paymentMethod)
        {
            var user = await _userManager.GetUserAsync(User);
            var room = await _context.Rooms.FindAsync(roomId);

            if (room == null || user == null) return NotFound();

            // 1. Tạo đơn Đặt chỗ
            var booking = new Booking
            {
                RoomId = roomId,
                UserId = user.Id,
                TransactionType = transactionType,
                Amount = amount,
                Status = "Paid",
                PaymentMethod = paymentMethod,
                CreatedDate = DateTime.Now
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // 2. Tạo Hóa đơn
            var invoice = new Invoice
            {
                InvoiceCode = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                BookingId = booking.Id,
                CustomerName = user.FullName ?? user.UserName ?? "Khách hàng",
                CustomerPhone = user.PhoneNumber ?? "Chưa cập nhật",
                TotalAmount = amount,
                CreatedDate = DateTime.Now,
                Note = transactionType == "Buyout" ? $"Thanh toán MUA ĐỨT: {room.Title}" : $"Đặt cọc giữ chỗ phòng: {room.Title}"
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Chuyển hướng sang trang In hóa đơn
            return RedirectToAction(nameof(PrintInvoice), new { id = invoice.Id });
        }

        // GET: /Payment/PrintInvoice/1
        public async Task<IActionResult> PrintInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Booking)
                .ThenInclude(b => b!.Room)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null) return NotFound();

            return View(invoice);
        }
    }
}