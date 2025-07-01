using Graduation.Entities;
using Graduation.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Graduation.Services
{
    public class TomTomService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public TomTomService(IOptions<TomTomSettings> options)
        {
            _httpClient = new HttpClient();
            _apiKey = options.Value.ApiKey;
        }

        public async Task<(string address, string cityRegion)> GetAddressFromCoordinates(Location location)
        {
        var url = $"https://api.tomtom.com/search/2/reverseGeocode/{location.Latitude},{location.Longitude}.json?key={_apiKey}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        return ("Unknown location", "Unknown city");

        var json = await response.Content.ReadAsStringAsync();
        var data = JObject.Parse(json);

        var fullAddress = data["addresses"]?[0]?["address"]?["freeformAddress"]?.ToString() ?? "Unknown location";

        var city = data["addresses"]?[0]?["address"]?["municipality"]?.ToString() ?? "";
        var region = data["addresses"]?[0]?["address"]?["countrySubdivision"]?.ToString() ?? "";

        var cityRegion = $"{city}, {region}".Trim(',', ' ', '-');

        // Remove both city and region from full address
        var addressWithoutCityRegion = fullAddress
        .Replace(city, "")
        .Replace(region, "")
        .Trim(',', ' ', '-');

        return (addressWithoutCityRegion, cityRegion);
    }



        public async Task<(double distance, double duration)> GetDistanceAndTime(Location from, Location to)
        {
            var url = $"https://api.tomtom.com/routing/1/calculateRoute/{from.Latitude},{from.Longitude}:{to.Latitude},{to.Longitude}/json?key={_apiKey}&computeBestOrder=true&routeType=fastest";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return (0, 0);

            var json = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(json);

            var summary = data["routes"]?[0]?["summary"];
            if (summary == null) return (0, 0);

            double distance = summary.Value<double>("lengthInMeters") / 1000.0; // km
            double duration = summary.Value<double>("travelTimeInSeconds") / 60.0; // min

            return (distance, duration);
        }
    }
}
