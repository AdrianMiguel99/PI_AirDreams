using AirDreams.API.DTOs;

public interface IPdfService
{
    byte[] GenerateInvoice(
        ConfirmPurchaseDTO dto
    );

    byte[] GenerateItinerary(
        ConfirmPurchaseDTO dto
    );
}