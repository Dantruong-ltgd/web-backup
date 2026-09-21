namespace web_backup.Models
{
    public class Amenity
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = "User";

        // 📌 THÊM DÒNG NÀY ĐỂ LƯU ĐƯỜNG DẪN ẢNH ĐẠI DIỆN
        public string? AvatarUrl
        {
            get; set;
        }
        public ICollection<Room>? Rooms { get; set; }
    }
}
