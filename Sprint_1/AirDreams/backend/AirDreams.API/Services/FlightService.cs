using AirDreams.API.DTOs;
using AirDreams.ExternalAPI.DTOs;
using AirDreams.API.Models.Dtos;
using AirDreams.API.Repositories;
using AirDreams.API.Services.Interfaces;

namespace AirDreams.API.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IPartnerFlightService _partnerFlightService;
        private readonly IFlightConnectorService _flightConnectorService;

        public FlightService(IFlightRepository flightRepository, IPartnerFlightService partnerFlightService, IFlightConnectorService flightConnectorService)
        {
            _flightRepository = flightRepository;
            _partnerFlightService = partnerFlightService;
            _flightConnectorService = flightConnectorService;
        }

        public async Task<List<FlightItineraryDTO>> SearchFlightsAsync(
            string origin,
            string destination,
            DateTime earliestDeparture,
            DateTime latestDeparture,
            int quantityOfPassengers,
            bool includeStops
        )
        {
            ValidateParameters(origin, destination, earliestDeparture, latestDeparture, quantityOfPassengers);

            origin = origin.Trim().ToUpper();
            destination = destination.Trim().ToUpper();

            var result = new List<FlightItineraryDTO>();

            var directFlights = await _flightRepository.SearchFlightsAsync(
                origin,
                destination,
                earliestDeparture,
                latestDeparture,
                quantityOfPassengers
            );

            IEnumerable<dynamic> oneStopFlightsList = Enumerable.Empty<dynamic>();

            if (includeStops){
                    oneStopFlightsList = await _flightRepository.SearchOneStopFlightsAsync(
                    origin,
                    destination,
                    earliestDeparture,
                    latestDeparture,
                    quantityOfPassengers
                );

                var internalOriginFlights = await _flightRepository.SearchFlightsByOriginAsync(
                    origin,
                    earliestDeparture,
                    latestDeparture,
                    quantityOfPassengers
                );

                if (internalOriginFlights.Any())
                {
                    var earliestArrival = internalOriginFlights
                        .Min(f => (DateTime)f.ArrivalDateTime);

                    var latestArrival = internalOriginFlights
                        .Max(f => (DateTime)f.ArrivalDateTime);

                    var earliestExternalDeparture = earliestArrival.AddHours(2);
                    var latestExternalDeparture = latestArrival.AddHours(12);

                    var externalFlights = await _partnerFlightService.SearchPartnerFlightsAsync(
                        destination,
                        earliestExternalDeparture,
                        latestExternalDeparture,
                        quantityOfPassengers
                    );

                    if(externalFlights.Any())
                    {
                        var internalSegments = internalOriginFlights
                        .Select(f => (FlightSegmentDTO)MapToFlightSegment(f))
                        .ToList();

                        var connectedFlights = _flightConnectorService.ConnectFlights(
                            internalSegments,
                            externalFlights
                        );

                        var connectedItineraries = connectedFlights
                            .Select(c => MapConnectedFlightToItinerary(c))
                            .ToList();

                        result.AddRange(connectedItineraries);
                    }
                }
            }

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

        public async Task<List<ExternalResponseFlightDTO>> SearchFlightsByDestinationAsync(
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

            var result = new List<ExternalResponseFlightDTO>();

            foreach (var flight in flights)
            {
                var flightDto = MapToExternalResponseFlight(flight);
                result.Add(flightDto);
            }

            return result.ToList();
        }

        private void ValidateParameters(string origin, string destination, DateTime earliestDeparture, DateTime latestDeparture, int quantityOfPassengers)
        {
            if (string.IsNullOrWhiteSpace(origin) ||
                string.IsNullOrWhiteSpace(destination) ||
                quantityOfPassengers < 1 ||
                quantityOfPassengers > 10)
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
            var segment = MapToFlightSegment(flight);

            return new FlightItineraryDTO
            {
                ItineraryId = flight.FlightNumber,
                Stops = 0,
                TouristPrice = segment.TouristPrice,
                FirstClassPrice = segment.FirstClassPrice,
                Segments = new List<FlightSegmentDTO> { segment }
            };
        }

        private FlightItineraryDTO MapOneStopFlightToItineraryDTO(dynamic flight)
        {
            return new FlightItineraryDTO
            {
                ItineraryId = $"{flight.FirstFlightNumber}-{flight.SecondFlightNumber}",
                Stops = 1,
                TouristPrice = flight.TouristPrice,
                FirstClassPrice = flight.FirstClassPrice,
                Segments = new List<FlightSegmentDTO>
                {
                    new FlightSegmentDTO
                    {
                        FlightNumber = flight.FirstFlightNumber,
                        RouteId = flight.FirstRouteID,
                        DepartureDate = flight.FirstDepartureDate,
                        DepartureTime = flight.FirstDepartureTime,
                        ArrivalDate = flight.FirstArrivalDate,
                        ArrivalTime = flight.FirstArrivalTime,
                        CarryOnPrice = flight.FirstCarryOnPrice,
                        CheckedPrice = flight.FirstCheckedPrice,
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
                        FlightNumber = flight.SecondFlightNumber,
                        RouteId = flight.SecondRouteID,
                        DepartureDate = flight.SecondDepartureDate,
                        DepartureTime = flight.SecondDepartureTime,
                        ArrivalDate = flight.SecondArrivalDate,
                        ArrivalTime = flight.SecondArrivalTime,
                        CarryOnPrice = flight.SecondCarryOnPrice,
                        CheckedPrice = flight.SecondCheckedPrice,
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

        private ExternalResponseFlightDTO MapToExternalResponseFlight(dynamic flight)
        {
            return new ExternalResponseFlightDTO
            {
                flightGUID = flight.FlightNumber,
                departureTime = flight.DepartureTime,
                arrivalTime = flight.ArrivalTime,
                duration = flight.Duration != null ? ((TimeSpan)flight.Duration).ToString(@"hh\-mm") : string.Empty,
                departureAirport = new ExternalResponseAirportDTO
                {
                    code = flight.DepartureAirportCode,
                    name = flight.DepartureAirportName,
                    city = flight.DepartureCity
                },
                arrivalAirport = new ExternalResponseAirportDTO
                {
                    code = flight.ArrivalAirportCode,
                    name = flight.ArrivalAirportName,
                    city = flight.ArrivalCity
                },
                touristPrice = flight.TouristPrice,
                firstClassPrice = flight.FirstClassPrice,
                carryOnPrice = flight.CarryOnPrice,
                checkedPrice = flight.CheckedPrice
            };
        }

        private FlightSegmentDTO MapToFlightSegment(dynamic flight)
        {
            return new FlightSegmentDTO
            {
                FlightNumber = flight.FlightNumber,
                RouteId = flight.RouteId,
                DepartureDate = flight.DepartureDate,
                ArrivalDate = flight.ArrivalDate,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                CarryOnPrice = flight.CarryOnPrice,
                CheckedPrice = flight.CheckedPrice,
                TouristPrice = flight.TouristPrice,
                FirstClassPrice = flight.FirstClassPrice,
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
            };
        }

        private FlightItineraryDTO MapConnectedFlightToItinerary(ConnectedFlightDto connected)
        {
            var departureDateTime = DateTime.Parse(connected.ExternalFlight.departureTime);
            var arrivalDateTime = DateTime.Parse(connected.ExternalFlight.arrivalTime);

            var externalSegment = new FlightSegmentDTO
            {
                FlightNumber = connected.ExternalFlight.flightGUID,
                DepartureDate = departureDateTime.Date,
                DepartureTime = departureDateTime.ToString("HH:mm:ss"),
                ArrivalDate = arrivalDateTime.Date,
                ArrivalTime = arrivalDateTime.ToString("HH:mm:ss"),
                CarryOnPrice = connected.ExternalFlight.carryOnPrice,
                CheckedPrice = connected.ExternalFlight.checkedPrice,
                DepartureAirport = new AirportDTO
                {
                    Code = connected.ExternalFlight.departureAirport.code,
                    Name = connected.ExternalFlight.departureAirport.name,
                    City = connected.ExternalFlight.departureAirport.city
                },
                ArrivalAirport = new AirportDTO
                {
                    Code = connected.ExternalFlight.arrivalAirport.code,
                    Name = connected.ExternalFlight.arrivalAirport.name,
                    City = connected.ExternalFlight.arrivalAirport.city
                }
            };

            return new FlightItineraryDTO
            {
                ItineraryId = $"{connected.InternalFlight.FlightNumber}-{connected.ExternalFlight.flightGUID}",
                Stops = 1,
                TouristPrice = connected.TouristPrice,
                FirstClassPrice = connected.FirstClassPrice,
                Segments = new List<FlightSegmentDTO> { connected.InternalFlight, externalSegment }
            };
        }
    }
}
