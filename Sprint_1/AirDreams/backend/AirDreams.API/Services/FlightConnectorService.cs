using AirDreams.ExternalAPI.DTOs;
using AirDreams.API.DTOs;


public class FlightConnectorService : IFlightConnectorService
{
    private const int MinLayoverHours = 2; 
    private const int MaxLayoverHours = 12;

    public List<ConnectedFlightDTO> ConnectFlights(
        IEnumerable<FlightSegmentDTO> internalFlights,
        IEnumerable<ExternalResponseFlightDTO> externalFlights)
    {
        var results = new List<ConnectedFlightDTO>();

        foreach (var internalFlight in internalFlights)
        {
            
                if (!internalFlight.ArrivalDate.HasValue)
                    {
                        continue;
                    }


                var internalArrival = CombineDateAndTime(
                    internalFlight.ArrivalDate.Value,
                    internalFlight.ArrivalTime
                );

            foreach (var externalFlight in externalFlights)
            {
                if (internalFlight.ArrivalAirport.Code != externalFlight.departureAirport.code)
                {
                    continue;
                }

                if (!DateTime.TryParse(externalFlight.departureTime, out var externalDeparture))
                {
                    continue;
                }

                var layover = externalDeparture - internalArrival;

                if (layover.TotalHours < MinLayoverHours || layover.TotalHours > MaxLayoverHours)
                {
                    continue;
                }

                results.Add(new ConnectedFlightDTO
                {
                    InternalFlight = internalFlight,
                    ExternalFlight = externalFlight,
                    LayoverHours = layover.TotalHours,
                    TouristPrice = internalFlight.TouristPrice + externalFlight.touristPrice,
                    FirstClassPrice = internalFlight.FirstClassPrice + externalFlight.firstClassPrice
                });
            }
        }

        return results;
    }

    private DateTime CombineDateAndTime(DateTime date, string time)
    {
        var timeSpan = TimeSpan.Parse(time);
        return date.Date + timeSpan;
    }
}