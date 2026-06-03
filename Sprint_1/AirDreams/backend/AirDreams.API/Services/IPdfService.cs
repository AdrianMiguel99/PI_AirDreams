using AirDreams.API.Models.Dtos;

public interface IPdfService
{
    byte[] GenerateInvoice(
        ConfirmPurchaseDto dto
    );

    byte[] GenerateItinerary(
        ConfirmPurchaseDto dto
    );
}