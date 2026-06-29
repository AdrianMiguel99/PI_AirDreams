using AirDreams.API.Models.Dtos;
using AirDreams.API.Repositories;
using AirDreams.API.Services;
using AirDreams.API.Services.Interfaces;
using AirDreams.ExternalAPI.DTOs;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IPdfService _pdfService;
    private readonly IPartnerFlightService _partnerFlightService;
    private readonly IExternalFlightService _externalFlightService;

    public PurchaseService(
        IPurchaseRepository repository,
        IEmailService emailService,
        IPdfService pdfService,
        IPartnerFlightService partnerFlightService,
        IExternalFlightService externalFlightService)
        {
            _repository = repository;
            _emailService = emailService;
            _pdfService = pdfService;
            _partnerFlightService = partnerFlightService;
            _externalFlightService = externalFlightService;
        }
    public async Task<bool> CheckFlightAvailabilityAsync( string numberFlight, string seatClass, int requestedSeats)
    {
        return await _repository.CheckFlightAvailabilityAsync(
            numberFlight,
            seatClass,
            requestedSeats
        );
    }

    public async Task<PaymentResponseDto> ConfirmPurchaseAsync(ConfirmPurchaseDto dto)
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

        return new PaymentResponseDto
        {
            Success = true,
            Message = "Compra confirmada y pago procesado correctamente."
        };
    }

    private static decimal ComputeGeometricTotal(decimal basePrice, decimal multiplier, int quantity)
    {
        if (quantity <= 0) return 0;
        if (multiplier == 0) return basePrice * quantity;
        return basePrice * ((decimal)Math.Pow(1 + (double)multiplier, quantity) - 1) / multiplier;
    }
}