using AirDreams.ExternalAPI.DTOs;
using AirDreams.API.DTOs;


public interface IFlightConnectorService
{
    List<ConnectedFlightDTO> ConnectFlights(
        IEnumerable<FlightSegmentDTO> internalFlights,
        IEnumerable<ExternalResponseFlightDTO> externalFlights
    );
}