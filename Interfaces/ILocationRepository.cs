using Graduation.Entities;

namespace Graduation.Interfaces
{
    public interface ILocationRepository
    {
        Task<AppUser?> GetBlindBySightId(int sightId);
        Task<Location?> GetLocationForUser(int userId);
        void UpdateLocation(Location location);
        Task<bool> SaveUserLocation(int userId, double latitude, double longitude);
        Task<bool> SaveAllAsync();
    }
}

