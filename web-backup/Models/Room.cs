using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_backup.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } // Giá thuê hàng tháng
        public double Area { get; set; }
        public string Address { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        public bool IsOwner { get; set; }      // Badge: Chính chủ
        public bool HasMezzanine { get; set; } // Badge: Gác lửng

        // 📌 THÊM: ĐÁNH DẤU YÊU THÍCH -> TỰ ĐỘNG HIỂN THỊ Ở DÒNG "PHÒNG NỔI BẬT" TRANG CHỦ
        public bool IsFeatured { get; set; } = false;

        // 📌 BỔ SUNG: TRẠNG THÁI ĐÃ THUÊ / MUA (ĐỂ KHẮC PHỤC LỖI CS1061)
        public bool IsRented { get; set; } = false;

        // 📌 THÊM: MUA ĐỨT (ÁP DỤNG CHO DÀNH CHO CHUNG CƯ VÀ NHÀ NGUYÊN CĂN)
        public bool IsForSale { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalePrice { get; set; } // Giá mua đứt

        // Danh mục phòng (Chung cư, Nhà nguyên căn, Phòng trọ...)
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // 📌 THÊM: LIÊN KẾT CHỦ PHÒNG NẾU CẦN XỬ LÝ DÒNG TIỀN/LÊN HÓA ĐƠN
        public string? OwnerId { get; set; }
        public ApplicationUser? Owner { get; set; }

        public ICollection<Amenity>? Amenities { get; set; }
    }
}