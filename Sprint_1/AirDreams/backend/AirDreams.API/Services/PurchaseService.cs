using AirDreams.API.Models.Dtos;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IPdfService _pdfService;

    public PurchaseService(
        IPurchaseRepository repository,
        IEmailService emailService,
        IPdfService pdfService)
        {
            _repository = repository;
            _emailService = emailService;
            _pdfService = pdfService;
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

        await _repository.ConfirmPurchaseAsync(dto, lastFour);

        // generar PDFs
        var invoicePdf =
            _pdfService.GenerateInvoice(dto);

        var itineraryPdf =
            _pdfService.GenerateItinerary(dto);

        // enviar correo
        var email =
            dto.Passengers
                .FirstOrDefault()
                ?.EmailPassenger;

        if (!string.IsNullOrWhiteSpace(email))
        {
            await _emailService
                .SendPurchaseConfirmationEmail(
                    email,
                    dto,
                    invoicePdf,
                    itineraryPdf
                );
        }

        return new PaymentResponseDto
        {
            Success = true,
            Message =
                //"Correo enviado (prueba)"
               "Compra confirmada, pago procesado y correo enviado."
        };
    }
}