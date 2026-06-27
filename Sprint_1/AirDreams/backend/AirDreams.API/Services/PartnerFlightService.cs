using System.Text.Json;
using AirDreams.API.Models;
using AirDreams.API.Services.Interfaces;
using AirDreams.ExternalAPI.DTOs;
using Microsoft.Extensions.Options;

namespace AirDreams.API.Services
{
    public class PartnerFlightService : IPartnerFlightService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly List<PartnerApiSettings> _partners;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PartnerFlightService(
            IHttpClientFactory httpClientFactory,
            IOptions<List<PartnerApiSettings>> partnerOptions)
        {
            _httpClientFactory = httpClientFactory;
            _partners = partnerOptions.Value;
        }

        public async Task<List<ExternalResponseFlightDTO>> SearchPartnerFlightsAsync(
            string destination,
            DateTime earliestDeparture,
            DateTime latestDeparture,
            int quantityOfPassengers
        )
        {
            var tasks = _partners.Select(partner =>
                SearchSinglePartnerAsync(partner, destination, earliestDeparture, latestDeparture, quantityOfPassengers)
            );

            var resultsPerPartner = await Task.WhenAll(tasks);

            return resultsPerPartner.SelectMany(flights => flights).ToList();
        }

        private async Task<List<ExternalResponseFlightDTO>> SearchSinglePartnerAsync(
            PartnerApiSettings partner,
            string destination,
            DateTime earliestDeparture,
            DateTime latestDeparture,
            int quantityOfPassengers
        )
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AirDreams");
                client.Timeout = TimeSpan.FromSeconds(10);

                var query = $"{partner.BaseUrl}" +
                            $"?destination={Uri.EscapeDataString(destination)}" +
                            $"&earliestDeparture={earliestDeparture:yyyy-MM-ddTHH:mm}" +
                            $"&latestDeparture={latestDeparture:yyyy-MM-ddTHH:mm}" +
                            $"&quantityOfPassengers={quantityOfPassengers}" +
                            $"&apiKey={Uri.EscapeDataString(partner.ApiKey)}";

                var response = await client.GetAsync(query);

                if (!response.IsSuccessStatusCode)
                {
                    return new List<ExternalResponseFlightDTO>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var parsed = JsonSerializer.Deserialize<ExternalFlightResponse>(content, JsonOptions);

                return parsed?.Flights ?? new List<ExternalResponseFlightDTO>();
            }
            catch (Exception ex)
            {
                return new List<ExternalResponseFlightDTO>();
            }
        }
    }
}