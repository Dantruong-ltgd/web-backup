using System.ComponentModel.DataAnnotations;

namespace web_backup.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Quận quản lý (Dành cho Chủ trọ)")]
        public string? District { get; set; }
    }
}