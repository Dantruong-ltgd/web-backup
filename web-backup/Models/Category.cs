namespace web_backup.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Room>? Rooms { get; set; }
    }
}
