using AirDreams.API.DTOs;

namespace AirDreams.API.Services.Interfaces
{
    public interface IExternalFlightService
    {
        Task<List<FlightDTO>> SearchFlightsAsync(string origin, string destination, string earliestDeparture, string latestDeparture, int quantityOfPassengers, string apiKey);
    }
}
