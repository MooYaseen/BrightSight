namespace Graduation.Entities
{
    public class Location
    {
        public int Id { get; set; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
