using QuestPDF.Fluent;
using AirDreams.API.Models.Dtos;

public class PdfService : IPdfService
{
    public byte[] GenerateInvoice(
        ConfirmPurchaseDto dto
    )
    {
        return Document.Create(c =>
        {
            c.Page(page =>
            {
                page.Content()
                    .Column(col =>
                    {
                        col.Item()
                            .Text("FACTURA");

                        col.Item()
                            .Text(
                                dto.TransactionId
                            );

                        col.Item()
                            .Text(
                                dto.BuyerName
                            );

                        col.Item()
                            .Text(
                                $"Pasajeros: {dto.PassengerCount}"
                            );
                    });
            });
        }).GeneratePdf();
    }

    public byte[] GenerateItinerary(
        ConfirmPurchaseDto dto
    )
    {
        return Document.Create(c =>
        {
            c.Page(page =>
            {
                page.Content()
                    .Column(col =>
                    {
                        col.Item()
                            .Text(
                                "ITINERARIO"
                            );

                        foreach (
                            var s
                            in dto.Segments
                        )
                        {
                            col.Item()
                                .Text(
                                    s.FlightNumber
                                );
                        }
                    });
            });
        }).GeneratePdf();
    }
}