using AirDreams.ExternalAPI.DTOs;
using AirDreams.API.DTOs;
using AirDreams.API.Models.Dtos;

public interface IFlightConnectorService
{
    List<ConnectedFlightDto> ConnectFlights(
        IEnumerable<FlightSegmentDTO> internalFlights,
        IEnumerable<ExternalResponseFlightDTO> externalFlights
    );
}