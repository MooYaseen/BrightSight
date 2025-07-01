namespace Graduation.DTOs
{
    public class LocationDetailsDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; }
    public string City { get; set; }      // ➕ المدينة
    public double DistanceKm { get; set; }
    public double DurationMinutes { get; set; }
}

}
