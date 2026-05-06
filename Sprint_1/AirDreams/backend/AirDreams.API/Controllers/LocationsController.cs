using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationsController : ControllerBase
    {
        private static List<CountryEntry>? _countries;

        private List<CountryEntry> LoadCountries()
        {
            if (_countries != null) return _countries;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "countriesData.json");
            var json = System.IO.File.ReadAllText(filePath);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var list = new List<CountryEntry>();
            foreach (var element in root.EnumerateArray())
            {
                string name = element.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";
                var cities = new List<string>();
                if (element.TryGetProperty("cities", out var citiesProp))
                {
                    foreach (var city in citiesProp.EnumerateArray())
                        cities.Add(city.GetString() ?? "");
                }
                list.Add(new CountryEntry { Name = name, Cities = cities });
            }

            _countries = list;
            return _countries;
        }

        [HttpGet("countries")]
        public IActionResult GetCountries()
        {
            var countries = LoadCountries();
            var countryNames = countries.Select(c => c.Name).ToList();
            return Ok(countryNames);
        }

        [HttpGet("cities")]
        public IActionResult GetCities([FromQuery] string country)
        {
            if (string.IsNullOrWhiteSpace(country)) return BadRequest("Especifique un país.");
            var countries = LoadCountries();
            var entry = countries.FirstOrDefault(c =>
                c.Name.Equals(country, StringComparison.OrdinalIgnoreCase));
            return Ok(entry?.Cities ?? new List<string>());
        }

        private class CountryEntry
        {
            public string Name { get; set; } = string.Empty;
            public List<string> Cities { get; set; } = new List<string>();
        }
    }
}