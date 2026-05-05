using AirDreams.API.DTOs;
using AirDreams.API.Repositories.Interfaces;

namespace AirDreams.API.Services.Interfaces
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

            var flights = await _flightRepository.SearchFlightsAsync(
                origin.ToUpper(),
                destination.ToUpper(),
                earliestDeparture.TimeOfDay,
                latestDeparture.TimeOfDay,
                quantityOfPassengers
            );

            return flights.Select(MapToFlightDTO).ToList();
        }

        public async Task ValidateApiKeyAsync(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new UnauthorizedAccessException("INVALID_API_KEY: La API key es requerida.");
            }

            var airline = await _flightRepository.ValidateApiKeyAsync(apiKey);

            if (string.IsNullOrEmpty(airline))
            {
                throw new UnauthorizedAccessException("INVALID_API_KEY: La API key proporcionada no es válida.");
            }
        }

        private void ValidateParameters(string origin, string destination, DateTime earliestDeparture, DateTime latestDeparture, int quantityOfPassengers)
        {
            if (string.IsNullOrEmpty(origin) ||
                string.IsNullOrEmpty(destination) ||
                string.IsNullOrEmpty(earliestDeparture) ||
                string.IsNullOrEmpty(latestDeparture) ||
                quantityOfPassengers < 1)
            {
                throw new ArgumentException("Los parámetros de búsqueda son inválidos o faltan.");
            }

            if (origin.Length != 3 || destination.Length != 3)
            {
                throw new ArgumentException("Los códigos de aeropuerto deben tener exactamente 3 caracteres.");
            }

            if (origin.Equals(destination, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("El aeropuerto de origen y destino no pueden ser el mismo.");
            }

            if (earliestDeparture > latestDeparture)
            {
                throw new ArgumentException("El rango de fechas es inválido.");
            }
        }

        private FlightDTO MapToFlightDTO(dynamic flight)
        {
            return new FlightDTO
            {
                FlightGUID = flight.FlightGUID,
                CarryOnPrice = flight.CarryOnPrice,
                CheckedPrice = flight.CheckedPrice,
                Route = new RouteDTO
                {
                    Id = flight.FlightNumber,
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