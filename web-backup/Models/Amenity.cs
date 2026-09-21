namespace web_backup.Models
{
    public class Amenity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty; // Class Icon Bootstrap/FontAwesome
        public ICollection<Room>? Rooms { get; set; }
    }
}
