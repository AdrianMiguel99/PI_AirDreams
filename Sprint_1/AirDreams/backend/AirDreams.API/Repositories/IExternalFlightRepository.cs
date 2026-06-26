using AirDreams.ExternalAPI.DTOs;

public interface IExternalFlightRepository
{
    Task RegisterExternalFlightAsync(
        string transactionId,
        ExternalResponseFlightDTO flight,
        string partnerName);

    Task<bool> ExistsAsync(string flightNumber);
}