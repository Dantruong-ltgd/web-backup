using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_backup.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public string InvoiceCode { get; set; } = string.Empty; // Ví dụ: HD20260819001

        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string Note { get; set; } = string.Empty;
    }
}