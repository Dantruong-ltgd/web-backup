using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using web_backup.Data;
using web_backup.Models;

namespace web_backup.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GIAO DIỆN CHECKOUT / THANH TOÁN DÀNH CHO PHÒNG
        [HttpGet]
        public async Task<IActionResult> Checkout(int roomId, string type = "Deposit")
        {
            var room = await _context.Rooms
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
            {
                return NotFound("Không tìm thấy thông tin phòng yêu cầu.");
            }

            return View(room);
        }

        // 2. XỬ LÝ THANH TOÁN ĐẶT CỌC/MUA PHÒNG
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int RoomId, string TransactionType, string PaymentMethod)
        {
            var room = await _context.Rooms.FindAsync(RoomId);
            if (room == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2.1. Tạo bản ghi Booking
            var booking = new Booking
            {
                RoomId = RoomId,
                UserId = userId,
                CustomerName = User.Identity?.Name ?? "Khách hàng",
                CustomerPhone = "0332860710",
                BookingDate = DateTime.Now,
                Status = "ĐANG ĐỢI XÁC NHẬN TRÊN GMAIL"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // 2.2. Tính số tiền cọc
            decimal amount = (TransactionType == "Buyout")
                ? ((room.SalePrice ?? 0) * 0.05m)
                : (room.Price * 0.20m);

            string invoiceCode = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // 2.3. Tạo Hóa đơn
            var invoice = new Invoice
            {
                InvoiceCode = invoiceCode,
                BookingId = booking.Id,
                CustomerName = User.Identity?.Name ?? "Khách hàng",
                CustomerPhone = "0332860710",
                TotalAmount = amount,
                CreatedDate = DateTime.Now,
                Status = "ĐANG ĐỢI XÁC NHẬN TRÊN GMAIL"
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // 2.4. CỐ ĐỊNH CHỈ GỬI EMAIL VỀ DUY NHẤT BAPTRUONG2530@GMAIL.COM
            string targetEmail = "baptruong2530@gmail.com";

            string confirmUrl = Url.Action("ConfirmViaEmail", "Payment",
                new { invoiceCode = invoice.InvoiceCode }, Request.Scheme) ?? string.Empty;

            try
            {
                await SendConfirmationEmailAsync(targetEmail, invoice.InvoiceCode, amount, confirmUrl);
            }
            catch (Exception ex)
            {
                ViewBag.EmailError = "Lỗi gửi mail: " + ex.Message;
            }

            return View("PendingConfirmation", invoice);
        }

        // 💥 2.5. XỬ LÝ ĐĂNG KÝ MUA GÓI VIP (49.000đ / 7 NGÀY) QUA QR CODE
        [HttpPost]
        [Authorize(Roles = "Admin,ChuTro")]
        public async Task<IActionResult> ProcessVipPayment()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            decimal vipPrice = 49000;
            string invoiceCode = "VIP" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // Tạo Hóa đơn VIP
            var invoice = new Invoice
            {
                InvoiceCode = invoiceCode,
                CustomerName = user.FullName ?? user.UserName ?? "Chủ trọ",
                CustomerPhone = user.PhoneNumber ?? "0332860710",
                TotalAmount = vipPrice,
                CreatedDate = DateTime.Now,
                Status = "ĐANG ĐỢI XÁC NHẬN TRÊN GMAIL"
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Link xác nhận VIP gửi qua Mail
            string confirmUrl = Url.Action("ConfirmVipViaEmail", "Payment",
                new { invoiceCode = invoice.InvoiceCode, userId = user.Id }, Request.Scheme) ?? string.Empty;

            try
            {
                await SendConfirmationEmailAsync("baptruong2530@gmail.com", invoice.InvoiceCode, vipPrice, confirmUrl);
            }
            catch (Exception ex)
            {
                ViewBag.EmailError = "Lỗi gửi mail: " + ex.Message;
            }

            // Tạo VietQR tự động
            string bankId = "MB";
            string accountNo = "0332860710";
            string accountName = "TRUONG VAN BAP";
            string description = $"THANH TOAN VIP {invoiceCode}";

            ViewBag.QrUrl = $"https://img.vietqr.io/image/{bankId}-{accountNo}-compact2.png?amount={vipPrice}&addInfo={Uri.EscapeDataString(description)}&accountName={Uri.EscapeDataString(accountName)}";
            ViewBag.InvoiceCode = invoiceCode;
            ViewBag.Amount = vipPrice;

            return View("VipQrPayment");
        }

        // 3. XÁC NHẬN CỌC/MUA PHÒNG QUA LINK TRONG EMAIL
        [HttpGet]
        public async Task<IActionResult> ConfirmViaEmail(string invoiceCode)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceCode == invoiceCode);
            if (invoice != null)
            {
                invoice.Status = "ĐÃ THANH TOÁN";

                var booking = await _context.Bookings.FindAsync(invoice.BookingId);
                if (booking != null)
                {
                    booking.Status = "ĐÃ XÁC NHẬN";

                    // CẬP NHẬT TRẠNG THÁI PHÒNG THÀNH ĐÃ THUÊ/MUA (IsRented = true)
                    var room = await _context.Rooms.FindAsync(booking.RoomId);
                    if (room != null)
                    {
                        room.IsRented = true;
                    }
                }

                await _context.SaveChangesAsync();

                return Content(@"
                    <div style='text-align:center; padding-top:50px; font-family:Arial, sans-serif;'>
                        <h2 style='color:#16a34a;'>✓ Đã xác nhận thanh toán thành công!</h2>
                        <p style='color:#555;'>Cửa sổ này sẽ tự động đóng lại...</p>
                    </div>
                    <script>
                        setTimeout(function() { window.close(); }, 1200);
                    </script>", "text/html; charset=utf-8");
            }

            return Content("Mã hóa đơn không tồn tại hoặc giao dịch đã hết hạn.");
        }

        // 💥 3.1. XÁC NHẬN GÓI VIP QUA EMAIL -> KÍCH HOẠT 7 NGÀY VIP CHO USER
        [HttpGet]
        public async Task<IActionResult> ConfirmVipViaEmail(string invoiceCode, string userId)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceCode == invoiceCode);
            var user = await _context.Users.FindAsync(userId);

            if (invoice != null && user != null)
            {
                invoice.Status = "ĐÃ THANH TOÁN";

                // Tính toán thời hạn VIP (Cộng dồn nếu tài khoản đang còn hạn)
                DateTime baseDate = (user.IsVip && user.VipExpiryDate.HasValue && user.VipExpiryDate.Value > DateTime.Now)
                    ? user.VipExpiryDate.Value
                    : DateTime.Now;

                user.IsVip = true;
                user.VipExpiryDate = baseDate.AddDays(7);

                await _context.SaveChangesAsync();

                return Content(@"
                    <div style='text-align:center; padding-top:50px; font-family:Arial, sans-serif;'>
                        <h2 style='color:#16a34a;'>✓ Đã xác nhận thanh toán & Kích hoạt 7 ngày VIP thành công!</h2>
                        <p style='color:#555;'>Cửa sổ này sẽ tự động đóng lại...</p>
                    </div>
                    <script>
                        setTimeout(function() { window.close(); }, 1500);
                    </script>", "text/html; charset=utf-8");
            }

            return Content("Mã hóa đơn không tồn tại hoặc tài khoản không hợp lệ.");
        }

        // 3.2. API DÀNH CHO TRANG PENDING CONFIRMATION KIỂM TRA TRẠNG THÁI
        [HttpGet]
        public async Task<IActionResult> CheckStatus(string invoiceCode)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceCode == invoiceCode);
            if (invoice != null && invoice.Status == "ĐÃ THANH TOÁN")
            {
                return Json(new { isPaid = true, invoiceId = invoice.Id });
            }
            return Json(new { isPaid = false });
        }

        // 4. IN / XEM HÓA ĐƠN
        [HttpGet]
        public async Task<IActionResult> PrintInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Booking)
                .ThenInclude(b => b!.Room)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null) return NotFound("Hóa đơn không tồn tại.");

            return View(invoice);
        }

        // 5. HÀM GỬI EMAIL TỰ ĐỘNG
        private async Task SendConfirmationEmailAsync(string toEmail, string invoiceCode, decimal amount, string confirmUrl)
        {
            string fromEmail = "baptruong2530@gmail.com";
            string appPassword = "ncmdwiafrzjxwamm";

            var fromAddress = new MailAddress(fromEmail, "PT-SG SYSTEM");
            var toAddress = new MailAddress(toEmail);

            string emailBody = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f4f4f4;'>
                    <div style='max-width: 600px; margin: 0 auto; background: #fff; padding: 30px; border-radius: 10px;'>
                        <h2 style='color: #7c3aed; text-align: center;'>XÁC NHẬN THANH TOÁN GIAO DỊCH</h2>
                        <p>Xin chào Admin (<strong>{toEmail}</strong>),</p>
                        <p>Có một giao dịch mới cần xác nhận:</p>
                        <p>Mã hóa đơn: <strong style='color:#2563eb;'>{invoiceCode}</strong></p>
                        <p>Số tiền: <strong style='color:#dc2626;'>{amount:N0} VNĐ</strong></p>
                        <hr style='border: 0.5px solid #eee; margin: 20px 0;' />
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{confirmUrl}' style='background-color: #16a34a; color: white; padding: 14px 28px; text-decoration: none; border-radius: 25px; font-weight: bold; display: inline-block;'>
                                ✓ XÁC NHẬN HOÀN THÀNH THANH TOÁN
                            </a>
                        </div>
                    </div>
                </div>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, appPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = $"[PT-SG SYSTEM] Yêu cầu xác nhận thanh toán hóa đơn {invoiceCode}",
                Body = emailBody,
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(message);
            }
        }

        // 6. XEM BẢNG PHÂN TÍCH DOANH THU
        [HttpGet]
        public async Task<IActionResult> RevenueAnalysis(int? year)
        {
            int targetYear = year ?? DateTime.Now.Year;

            // Lấy toàn bộ hóa đơn đã hoàn tất thanh toán
            var paidInvoices = await _context.Invoices
                .Include(i => i.Booking)
                    .ThenInclude(b => b!.Room)
                        .ThenInclude(r => r!.Category)
                .Where(i => i.Status == "ĐÃ THANH TOÁN")
                .ToListAsync();

            // 1. Thống kê tổng quan
            var totalRevenue = paidInvoices.Sum(i => i.TotalAmount);
            var totalPaidInvoices = paidInvoices.Count;

            var now = DateTime.Now;
            var currentMonthRevenue = paidInvoices
                .Where(i => i.CreatedDate.Month == now.Month && i.CreatedDate.Year == now.Year)
                .Sum(i => i.TotalAmount);

            // 2. Doanh thu theo 12 tháng trong năm
            var monthlyRevenues = Enumerable.Range(1, 12).Select(m => {
                var itemsInMonth = paidInvoices.Where(i => i.CreatedDate.Year == targetYear && i.CreatedDate.Month == m).ToList();
                return new MonthlyRevenueItem
                {
                    Month = m,
                    Year = targetYear,
                    Total = itemsInMonth.Sum(i => i.TotalAmount),
                    Count = itemsInMonth.Count
                };
            }).ToList();

            // 3. Doanh thu theo danh mục
            var categoryRevenues = paidInvoices
                .Where(i => i.Booking?.Room?.Category != null)
                .GroupBy(i => i.Booking!.Room!.Category!.Name)
                .Select(g => new CategoryRevenueItem
                {
                    CategoryName = g.Key,
                    Total = g.Sum(i => i.TotalAmount),
                    Count = g.Count()
                })
                .OrderByDescending(c => c.Total)
                .ToList();

            // 4. Danh sách hóa đơn thành công gần đây (Top 10)
            var recentInvoices = paidInvoices
                .OrderByDescending(i => i.CreatedDate)
                .Take(10)
                .ToList();

            var model = new RevenueReportViewModel
            {
                TotalRevenue = totalRevenue,
                TotalPaidInvoices = totalPaidInvoices,
                CurrentMonthRevenue = currentMonthRevenue,
                MonthlyRevenues = monthlyRevenues,
                CategoryRevenues = categoryRevenues,
                RecentInvoices = recentInvoices
            };

            ViewBag.SelectedYear = targetYear;
            return View(model);
        }
    }
}