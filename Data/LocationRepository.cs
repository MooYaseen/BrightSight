using Graduation.Entities;
using Graduation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Graduation.Data
{
    public class LocationRepository : ILocationRepository
    {
        private readonly DataContext _context;
        public LocationRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<AppUser?> GetBlindBySightId(int sightId)
        {
            return await _context.UserConnections
                .Where(c => c.SightId == sightId)
                .Select(c => c.Blind)
                .FirstOrDefaultAsync();
        }

        public async Task<Location?> GetLocationForUser(int userId)
        {
            return await _context.Locations
                .FirstOrDefaultAsync(l => l.AppUserId == userId);
        }

        public void UpdateLocation(Location location)
        {
            _context.Entry(location).State = EntityState.Modified;
        }

        public async Task<bool> SaveUserLocation(int userId, double latitude, double longitude)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.AppUserId == userId);

            if (location == null)
            {
                location = new Location
                {
                    AppUserId = userId,
                    Latitude = latitude,
                    Longitude = longitude,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Locations.Add(location);
            }
            else
            {
                location.Latitude = latitude;
                location.Longitude = longitude;
                location.UpdatedAt = DateTime.UtcNow;
                _context.Locations.Update(location);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
