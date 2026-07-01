using AirDreams.API.DTOs;
using AirDreams.API.Repositories;
using AirDreams.API.Services;
using AirDreams.API.Services.Interfaces;
using AirDreams.ExternalAPI.DTOs;
using AirDreams.API.DTOs;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IPdfService _pdfService;
    private readonly IPartnerFlightService _partnerFlightService;
    private readonly IExternalFlightService _externalFlightService;
    private readonly IFlightService _flightService;
    private readonly ILuggageService _luggageService;
    private readonly IPassengerValidationService _passengerValidationService;

    public PurchaseService(
        IPurchaseRepository repository,
        IEmailService emailService,
        IPdfService pdfService,
        IPartnerFlightService partnerFlightService,
        IExternalFlightService externalFlightService,
        IFlightService flightService,
        ILuggageService luggageService)
        {
            _repository = repository;
            _emailService = emailService;
            _pdfService = pdfService;
            _partnerFlightService = partnerFlightService;
            _externalFlightService = externalFlightService;
            _flightService = flightService;
            _luggageService = luggageService;
        }
    public async Task<bool> CheckFlightAvailabilityAsync( string numberFlight, string seatClass, int requestedSeats)
    {
        return await _repository.CheckFlightAvailabilityAsync(
            numberFlight,
            seatClass,
            requestedSeats
        );
    }

    public async Task<PaymentResponseDTO> ConfirmPurchaseAsync(ConfirmPurchaseDTO dto)
    {
        string? lastFour = null;
        if (dto.PaymentMethod.Equals("Card", StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(dto.CardNumber))
        {
            var digits = new string(dto.CardNumber.Where(char.IsDigit).ToArray());
            lastFour = digits.Length >= 4 ? digits[^4..] : null;
        }

        var externalSegments = dto.Segments
            .Where(s => !s.FlightNumber.StartsWith("AD"))
            .ToList();

        var externalFlights = new List<ExternalResponseFlightDTO>();

        foreach (var segment in externalSegments)
        {
            var externalFlight =
                await _partnerFlightService.GetCachedFlightAsync(segment.FlightNumber);

            if (externalFlight != null)
            {
                externalFlights.Add(externalFlight);
            }
        }

        dto.Segments.RemoveAll(s => !s.FlightNumber.StartsWith("AD"));
        await _repository.ConfirmPurchaseAsync(dto, lastFour);

        foreach (var externalFlight in externalFlights)
        {
            await _externalFlightService.RegisterExternalFlightAsync(
                dto.TransactionId,
                externalFlight);
        }

        var flightNumbers = dto.Segments.Select(s => s.FlightNumber).Distinct();
        var multipliers = await _repository.GetMultipliersByFlightsAsync(flightNumbers);

        if (dto.Luggage != null)
        {
            foreach (var passengerLuggage in dto.Luggage)
            {
                foreach (var item in passengerLuggage.LuggageItems)
                {
                    decimal subtotal = 0;
                    foreach (var seg in dto.Segments)
                    {
                        if (!multipliers.TryGetValue(seg.FlightNumber, out var multiplier))
                            multiplier = 0.5m; 

                        decimal basePrice = item.Type == "checked" ? seg.CheckedPrice : seg.CarryOnPrice;
                        subtotal += ComputeGeometricTotal(basePrice, multiplier, item.Quantity);
                    }
                    item.Subtotal = subtotal;   
                }
            }
        }

        var email = dto.Passengers.FirstOrDefault()?.EmailPassenger;
        if (!string.IsNullOrWhiteSpace(email))
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var invoicePdf = _pdfService.GenerateInvoice(dto);
                    var itineraryPdf = _pdfService.GenerateItinerary(dto);
                    await _emailService.SendPurchaseConfirmationEmail(
                        email,
                        dto,
                        invoicePdf,
                        itineraryPdf
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error enviando correo: {ex.Message}");
                }
            });
        }

        return new PaymentResponseDTO
        {
            Success = true,
            Message = "Compra confirmada y pago procesado correctamente."
        };
    }

    public async Task<ExternalPaymentResponseDto> ConfirmExternalPurchaseAsync(ExternalOrderRequestDto dto)
    {        
        var purchase = await MapToConfirmPurchaseDto(dto);
        var seatClass = purchase.SeatClass == "Turista" ? "Turist" : purchase.SeatClass;

        bool available = await CheckFlightAvailabilityAsync(purchase.Segments.First().FlightNumber, seatClass, purchase.PassengerCount);

        if (!available)
        {
            throw new InvalidOperationException("Not enough seats");
        }

        decimal luggageWeight = dto.passengers.Sum(p => p.Checked * 23m);
        decimal carryOnWeight = dto.passengers.Sum(p => p.carryOn ? 10m : 0m);
        int routeId = await _flightService.GetRouteIdByFlightGuidAsync(dto.flightGUID);

        var luggageResult = await _luggageService.CheckAvailabilityAsync(dto.flightGUID, routeId, luggageWeight, carryOnWeight);

        if (!luggageResult.success)
        {
            throw new InvalidOperationException("Luggage overweight");
        }

        await ConfirmPurchaseAsync(purchase);
        return await MapToExternalPaymentResponse(purchase, dto);
    }

    private static decimal ComputeGeometricTotal(decimal basePrice, decimal multiplier, int quantity)
    {
        if (quantity <= 0) return 0;
        if (multiplier == 0) return basePrice * quantity;
        return basePrice * ((decimal)Math.Pow(1 + (double)multiplier, quantity) - 1) / multiplier;
    }

    private async Task<ConfirmPurchaseDTO> MapToConfirmPurchaseDto(ExternalOrderRequestDto dto)
    {
        var flight = await _flightService.GetFlightByGuidAsync(dto.flightGUID);

        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        return new ConfirmPurchaseDTO
        {
            TransactionId = $"TXN-{Guid.NewGuid().ToString("N")[..8]}",
            BuyerName = $"{dto.buyer.firstName} {dto.buyer.lastName}",
            PaymentMethod = "Card",
            CardNumber = dto.payment.cardNumber,
            CardExpiry = dto.payment.cardExpiration,
            CardCvv = dto.payment.cvv,
            SeatClass = dto.firstClass ? "FirstClass" : "Turista",
            PassengerCount = dto.passengers.Count,
            PricePerPassenger = dto.firstClass ? flight.FirstClassPrice : flight.TouristPrice,

            Segments = new()
{
            new FlightSegmentDTO
            {
                FlightNumber = flight.FlightGUID,
                DepartureDate = flight.DepartureDate,
                ArrivalDate = flight.ArrivalDate,
                DepartureTime = Convert.ToString(flight.DepartureTime),
                ArrivalTime = Convert.ToString(flight.ArrivalTime),
                Duration = flight.Duration,
                CheckedPrice = flight.CheckedPrice,
                CarryOnPrice = flight.CarryOnPrice,
                Multiplier = flight.Multiplier,

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
        },

            Passengers = dto.passengers.Select(p => new PassengerDTO
            {
                NamePassenger = p.firstName,

                LastnamesPassenger =
                    $"{p.lastName} {p.lastName2}".Trim(),

                EmailPassenger = dto.buyer.email,

                Telephone = Convert.ToInt64(dto.buyer.phoneNumber),

                Country = p.passportCountry,

                BirthDate = p.birthDate.ToString("yyyy-MM-dd")

            }).ToList(),

            Luggage = dto.passengers
                .Select((p, index) => new LuggagePerPassengerDto
                {
                    PassengerIndex = index + 1,

                    LuggageItems = new()
                    {
                        new LuggageItemDTO
                        {
                            Type = "carryOn",
                            Quantity = p.carryOn ? 1 : 0
                        },

                        new LuggageItemDTO
                        {
                            Type = "checked",
                            Quantity = p.Checked
                        }
                    }
                }).ToList()
        };
    }

    private async Task<ExternalPaymentResponseDto> MapToExternalPaymentResponse(ConfirmPurchaseDTO purchase, ExternalOrderRequestDto request)
    {
        var flight = purchase.Segments.First();
        var luggageTotal = purchase.Luggage?.SelectMany(l => l.LuggageItems).Sum(i => i.Subtotal) ?? 0;
        var ticketsTotal = purchase.PricePerPassenger * purchase.PassengerCount;
        var taxes = 0m;

        return new ExternalPaymentResponseDto
        {
            reservationNumber = purchase.TransactionId,
            firstClass = request.firstClass,

            flight = new ExternalFlightResponseDto
            {
                flightGUID = flight.FlightNumber,
                departureTime =
                    $"{flight.DepartureDate:yyyy-MM-dd}T{flight.DepartureTime}",
                arrivalTime =
                    $"{flight.ArrivalDate:yyyy-MM-dd}T{flight.ArrivalTime}",
                duration = Convert.ToString(flight.Duration),

                departureAirport = new ExternalAirportDto
                {
                    code = flight.DepartureAirport.Code,
                    name = flight.DepartureAirport.Name,
                    city = flight.DepartureAirport.City
                },

                arrivalAirport = new ExternalAirportDto
                {
                    code = flight.ArrivalAirport.Code,
                    name = flight.ArrivalAirport.Name,
                    city = flight.ArrivalAirport.City
                },

                touristPrice = request.firstClass ? 0 : purchase.PricePerPassenger,
                firstClassPrice = request.firstClass ? purchase.PricePerPassenger : 0,
                carryOnPrice = flight.CarryOnPrice,
                checkedPrice = flight.CheckedPrice
            },

            breakup = new ExternalBreakupDto
            {
                luggage = luggageTotal,
                tickets = ticketsTotal,
                taxes = taxes,
                total = ticketsTotal + luggageTotal + taxes
            },

            passengers = request.passengers,

            buyer = new ExternalBuyerResponseDto
            {
                firstName = request.buyer.firstName,
                lastName = request.buyer.lastName,
                lastName2 = request.buyer.lastName2,
                phoneNumber = request.buyer.phoneNumber,
                email = request.buyer.email
            }
        };
    }
}