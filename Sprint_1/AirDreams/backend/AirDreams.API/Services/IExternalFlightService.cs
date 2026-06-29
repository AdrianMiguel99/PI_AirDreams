using AirDreams.ExternalAPI.DTOs;

public interface IExternalFlightService
{
    Task RegisterExternalFlightAsync(
        string transactionId,
        ExternalResponseFlightDTO flight);
}