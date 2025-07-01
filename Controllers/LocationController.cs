using Graduation.DTOs;
using Graduation.Extensions;
using Graduation.Interfaces;
using Graduation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Graduation.Controllers
{
    [Authorize]
    public class LocationController : BaseApiController
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly TomTomService _tomtomService;

        public LocationController(ILocationRepository locationRepository, IUserRepository userRepository, TomTomService tomtomService)
        {
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _tomtomService = tomtomService;
        }

        [HttpGet("track-blind")]
        public async Task<ActionResult<LocationDetailsDto>> GetBlindLocation()
        {
            var sightUsername = User.GetUsername();
            var sightUser = await _userRepository.GetUserByUsernameAsync(sightUsername);
            if (sightUser == null) return Unauthorized();

            var blindUser = await _locationRepository.GetBlindBySightId(sightUser.Id);
            if (blindUser == null) return NotFound("No linked blind user");

            var blindLocation = await _locationRepository.GetLocationForUser(blindUser.Id);
            if (blindLocation == null) return NotFound("No location found for blind user");

            var sightLocation = await _locationRepository.GetLocationForUser(sightUser.Id);
            if (sightLocation == null) return NotFound("No location found for sight user");




            var (address, cityRegion) = await _tomtomService.GetAddressFromCoordinates(blindLocation);
            var (distance, duration) = await _tomtomService.GetDistanceAndTime(sightLocation, blindLocation);

            return new LocationDetailsDto
            {
                Latitude = blindLocation.Latitude,
                Longitude = blindLocation.Longitude,
                Address = address,
                City = cityRegion,
                DistanceKm = Math.Round(distance, 1),
                DurationMinutes = Math.Round(duration, 1)
            };
        }

        // ✅ ميثود إرسال الموقع من الكفيف
        [HttpPost("send-location")]
        public async Task<ActionResult> SendLocation(LocationDto locationDto)
        {
        var username = User.GetUsername();
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if (user == null) return Unauthorized();

        await _locationRepository.SaveUserLocation(user.Id, locationDto.Latitude, locationDto.Longitude);
    
        // 🟡 الإضافة دي كانت ناقصة
        await _locationRepository.SaveAllAsync();

        return Ok("Location saved");
        }


    }
}
