using AirDreams.API.DTOs;
using AirDreams.API.Repositories;
using AirDreams.API.Services.Interfaces;

namespace AirDreams.API.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;

        public FlightService(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }

        public async Task<List<FlightDTO>> SearchFlightsAsync(
            string origin,
            string destination,
            DateTime earliestDeparture,
            DateTime latestDeparture,
            int quantityOfPassengers
        )
        {
            ValidateParameters(origin, destination, earliestDeparture, latestDeparture, quantityOfPassengers);

            origin = origin.Trim().ToUpper();
            destination = destination.Trim().ToUpper();

            var flightsList = await _flightRepository.SearchFlightsAsync(
                origin,
                destination,
                earliestDeparture,
                latestDeparture,
                quantityOfPassengers
            );

            var result = new List<FlightDTO>();
            foreach (var flight in flightsList)
            {
                var flightDto = MapToFlightDTO(flight, origin, destination);
                result.Add(flightDto);
            }

            return result;
        }

        private void ValidateParameters(string origin, string destination, DateTime earliestDeparture, DateTime latestDeparture, int quantityOfPassengers)
        {
            if (string.IsNullOrWhiteSpace(origin) ||
                string.IsNullOrWhiteSpace(destination) ||
                quantityOfPassengers < 1)
            {
                throw new ArgumentException("INVALID_PARAMETERS: Parámetros inválidos o faltantes.");
            }

            if (origin.Length != 3 || destination.Length != 3)
            {
                throw new ArgumentException("INVALID_AIRPORT_CODE: Los códigos de aeropuerto deben tener exactamente 3 caracteres.");
            }

            if (origin.Equals(destination, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("SAME_AIRPORTS: El aeropuerto de origen y destino no pueden ser el mismo.");
            }

            if (earliestDeparture > latestDeparture)
            {
                throw new ArgumentException("INVALID_DATE_RANGE: El rango de fechas es inválido.");
            }
        }

        private FlightDTO MapToFlightDTO(dynamic flight, string origin, string destination)
        {
            return new FlightDTO
            {
                FlightGUID = flight.FlightNumber,
                DepartureDate = flight.DepartureDate,
                CarryOnPrice = flight.CarryOnPrice,
                CheckedPrice = flight.CheckedPrice,
                Route = new RouteDTO
                {
                    Id = flight.RouteId,
                    DepartureTime = flight.DepartureTime,
                    ArrivalTime = flight.ArrivalTime,
                    Duration = flight.Duration,
                    DepartureAirport = new AirportDTO
                    {
                        Code = flight.DepartureAirportCode,
                        Name = flight.DepartureAirportName,
                        City = flight.DepartureCity
                    },
                    ArrivalAirport = new AirportDTO
                    {
                        Code = flight.ArrivalAirportCode,
                        Name = flight.ArrivalAirportName,
                        City = flight.ArrivalCity
                    },
                    TouristPrice = flight.TouristPrice,
                    FirstClassPrice = flight.FirstClassPrice
                }
            };
        }
    }
}