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
            string earliestDeparture,
            string latestDeparture,
            int quantityOfPassengers
        )
        {
            ValidateParameters(origin, destination, earliestDeparture, latestDeparture, quantityOfPassengers);

            var (parsedEarliest, parsedLatest) = ValidateAndParseDates(earliestDeparture, latestDeparture);

            var flights = await _flightRepository.SearchFlightsAsync(
                origin.ToUpper(),
                destination.ToUpper(),
                parsedEarliest.TimeOfDay,
                parsedLatest.TimeOfDay,
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

        private void ValidateParameters(string origin, string destination, string earliestDeparture, string latestDeparture, int quantityOfPassengers)
        {
            if (string.IsNullOrEmpty(origin) ||
                string.IsNullOrEmpty(destination) ||
                string.IsNullOrEmpty(earliestDeparture) ||
                string.IsNullOrEmpty(latestDeparture) ||
                quantityOfPassengers < 1)
            {
                throw new ArgumentException(
                    "INVALID_PARAMETERS: Parámetros inválidos o faltantes."
                );
            }

            if (origin.Length != 3 || destination.Length != 3)
            {
                throw new ArgumentException(
                    "INVALID_AIRPORT_CODE: Los códigos de aeropuerto deben tener 3 caracteres."
                );
            }

            if (origin.Equals(destination, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "SAME_AIRPORTS: El aeropuerto de origen y destino no pueden ser el mismo."
                );
            }
        }

        private (DateTime earliest, DateTime latest) ValidateAndParseDates(string earliestDeparture, string latestDeparture)
        {
            if (!DateTime.TryParse(earliestDeparture, out DateTime parsedEarliest) ||
                !DateTime.TryParse(latestDeparture, out DateTime parsedLatest))
            {
                throw new ArgumentException(
                    "INVALID_DATE_FORMAT: Las fechas deben estar en formato ISO (YYYY-MM-DDThh:mm)."
                );
            }

            if (parsedEarliest > parsedLatest)
            {
                throw new ArgumentException(
                    "INVALID_DATE_RANGE: La fecha de salida más temprana debe ser antes o igual a la fecha de salida más tardía."
                );
            }

            return (parsedEarliest, parsedLatest);
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
