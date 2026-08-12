using Microsoft.AspNetCore.Identity;

namespace web_backup.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        // Quận/Huyện mà chủ trọ được phép quản lý (Ví dụ: "Bình Thạnh", "Quận 1")
        public string? District { get; set; }
    }
}