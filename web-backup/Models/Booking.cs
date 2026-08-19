using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_backup.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int RoomId { get; set; }
        public Room? Room { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // TransactionType: "Deposit" (Đặt cọc giữ chỗ 20%) hoặc "Buyout" (Mua đứt)
        public string TransactionType { get; set; } = "Deposit";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string Status { get; set; } = "Paid";
        public string PaymentMethod { get; set; } = "Chuyển khoản Ngân hàng";
    }
}