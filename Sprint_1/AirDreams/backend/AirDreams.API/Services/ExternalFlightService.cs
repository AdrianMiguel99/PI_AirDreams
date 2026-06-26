using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services.Interfaces;
using AirDreams.ExternalAPI.DTOs;

namespace AirDreams.API.Services
{
    public class ExternalFlightService : IExternalFlightService
    {
        private readonly IExternalFlightRepository _repository;

        private static readonly Dictionary<string, string> PartnerNames = new()
        {
            { "MU", "Mushu Airlines" },
            { "SN", "Snoopy Airlines" },
            { "ZU", "Zuli Airlines" }
        };

        public ExternalFlightService(IExternalFlightRepository repository)
        {
            _repository = repository;
        }

        public async Task RegisterExternalFlightAsync(
            string transactionId,
            ExternalResponseFlightDTO flight)
        {
            if (flight == null)
                throw new ArgumentNullException(nameof(flight));

            var prefix = flight.flightGUID.Length >= 2
                ? flight.flightGUID[..2]
                : "";

            var partnerName = PartnerNames.TryGetValue(prefix, out var partner)
                ? partner
                : "Unknown";

            await _repository.RegisterExternalFlightAsync(
                transactionId,
                flight,
                partnerName);
        }
    }
}