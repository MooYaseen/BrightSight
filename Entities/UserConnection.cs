using Graduation.Entities;
public class UserConnection
{
    public int Id { get; set; }

    public int BlindId { get; set; }
    public int SightId { get; set; }

    // علاقات الربط
    public AppUser Blind { get; set; }
    public AppUser Sight { get; set; }
}
