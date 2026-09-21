namespace web_backup.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Area { get; set; }
        public string Address { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsOwner { get; set; }      // Badge: Chính chủ
        public bool HasMezzanine { get; set; } // Badge: Gác lửng

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Amenity>? Amenities { get; set; }
    }
}
