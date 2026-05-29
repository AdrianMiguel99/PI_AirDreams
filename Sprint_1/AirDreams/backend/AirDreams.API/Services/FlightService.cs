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

        public async Task<List<FlightItineraryDTO>> SearchFlightsAsync(
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

            var directFlights = await _flightRepository.SearchFlightsAsync(
                origin,
                destination,
                earliestDeparture,
                latestDeparture,
                quantityOfPassengers
            );
            

            var oneStopFlightsList = await _flightRepository.SearchOneStopFlightsAsync(
                origin,
                destination,
                earliestDeparture,
                latestDeparture,
                quantityOfPassengers
            );
            
            var result = new List<FlightItineraryDTO>();

            foreach (var flight in directFlights)
            {
                var flightDto = MapToFlightItinerary(flight, origin, destination);
                result.Add(flightDto);
            }

            foreach (var flight in oneStopFlightsList)
            {
                var flightDto = MapOneStopFlightToItineraryDTO(flight);
                result.Add(flightDto);
            }

            return result
            .OrderBy(itinerary => itinerary.Stops)
            .ThenBy(itinerary => itinerary.TouristPrice)
            .ToList();
        }

        public async Task<List<FlightItineraryDTO>> SearchFlightsByDestinationAsync(
            string destination,
            DateTime earliestDeparture,
            DateTime latestDeparture,
            int quantityOfPassengers
        )
        {
            destination = destination.Trim().ToUpper();

            var flights = await _flightRepository.SearchFlightsByDestinationAsync(
                destination,
                earliestDeparture,
                latestDeparture,
                quantityOfPassengers
            );

            var result = new List<FlightItineraryDTO>();

            foreach (var flight in flights)
            {
                var flightDto = MapToFlightItinerary(flight, flight.DepartureAirportCode, flight.ArrivalAirportCode);
                result.Add(flightDto);
            }

            return result.ToList();
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

        private FlightItineraryDTO MapToFlightItinerary(dynamic flight, string origin, string destination)
        {
            return new FlightItineraryDTO
            {
                ItineraryId = flight.FlightNumber,
                Stops = 0,
                TouristPrice = flight.TouristPrice,
                FirstClassPrice = flight.FirstClassPrice,
                Segments = new List<FlightSegmentDTO>
                {
                    new FlightSegmentDTO
                    {
                        RouteId = flight.RouteId,
                        DepartureDate = flight.DepartureDate,
                        ArrivalDate = flight.ArrivalDate,
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
                        }
                    }
                }
            };
        }
                private FlightItineraryDTO MapOneStopFlightToItineraryDTO(dynamic flight)
        {
            return new FlightItineraryDTO
            {
                ItineraryId = flight.FlightNumber,
                Stops = 1,
                TouristPrice = flight.TouristPrice,
                FirstClassPrice = flight.FirstClassPrice,
                Segments = new List<FlightSegmentDTO>
                {
                    new FlightSegmentDTO
                    {
                        RouteId = flight.FirstRouteID,
                        DepartureDate = flight.FirstDepartureDate,
                        DepartureTime = flight.FirstDepartureTime,
                        ArrivalDate = flight.FirstArrivalDate,
                        ArrivalTime = flight.FirstArrivalTime,
                        Duration = flight.FirstDuration,
                        DepartureAirport = new AirportDTO
                        {
                            Code = flight.FirstDepartureAirportCode,
                            Name = flight.FirstDepartureAirportName,
                            City = flight.FirstDepartureCity
                        },
                        ArrivalAirport = new AirportDTO
                        {
                            Code = flight.FirstArrivalAirportCode,
                            Name = flight.FirstArrivalAirportName,
                            City = flight.FirstArrivalCity
                        }
                    },
                    new FlightSegmentDTO
                    {
                        RouteId = flight.SecondRouteID,
                        DepartureDate = flight.SecondDepartureDate,
                        DepartureTime = flight.SecondDepartureTime,
                        ArrivalDate = flight.SecondArrivalDate,
                        ArrivalTime = flight.SecondArrivalTime,
                        Duration = flight.SecondDuration,
                        DepartureAirport = new AirportDTO
                        {
                            Code = flight.FirstArrivalAirportCode,
                            Name = flight.FirstArrivalAirportName,
                            City = flight.FirstArrivalCity
                        },
                        ArrivalAirport = new AirportDTO
                        {
                            Code = flight.FinalArrivalAirportCode,
                            Name = flight.FinalArrivalAirportName,
                            City = flight.FinalArrivalCity
                        }
                    }
                }
            };
        }
    }
}