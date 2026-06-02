using QuestPDF.Fluent;
using AirDreams.API.Models.Dtos;

public class PdfService : IPdfService
{
    public byte[] GenerateInvoice(ConfirmPurchaseDto dto)
    {
        decimal luggageTotal = 0;

        if (dto.Luggage != null)
        {
            luggageTotal =
                dto.Luggage
                    .SelectMany(x => x.LuggageItems)
                    .Sum(x => x.Subtotal);
        }

        decimal flightTotal =
            dto.PricePerPassenger *
            dto.PassengerCount;

        decimal grandTotal =
            flightTotal + luggageTotal;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .Text("AIRDREAMS")
                    .FontSize(24)
                    .Bold();

                page.Content()
                    .Column(column =>
                    {
                        column.Spacing(10);

                        column.Item()
                            .Text("FACTURA DE COMPRA")
                            .FontSize(18)
                            .Bold();

                        column.Item().Text(
                            $"Transacción: {dto.TransactionId}");

                        column.Item().Text(
                            $"Comprador: {dto.BuyerName}");

                        column.Item().Text(
                            $"Método de pago: {dto.PaymentMethod}");

                        column.Item().Text(
                            $"Clase: {dto.SeatClass}");

                        column.Item().Text(
                            $"Pasajeros: {dto.PassengerCount}");

                        column.Item().PaddingTop(15);

                        column.Item()
                            .Text("PASAJEROS")
                            .Bold();

                        foreach (var passenger in dto.Passengers)
                        {
                            column.Item()
                                .Text(
                                    $"{passenger.NamePassenger} {passenger.LastnamesPassenger}"
                                );
                        }

                        if (dto.Luggage != null &&
                            dto.Luggage.Any())
                        {
                            column.Item()
                                .PaddingTop(15);

                            column.Item()
                                .Text("EQUIPAJE")
                                .Bold();

                            foreach (var luggage in dto.Luggage)
                            {
                                foreach (var item in luggage.LuggageItems)
                                {
                                    column.Item()
                                        .Text(
                                            $"{item.Type} x{item.Quantity} - ${item.Subtotal}"
                                        );
                                }
                            }
                        }

                        column.Item()
                            .PaddingTop(20);

                        column.Item()
                            .Text(
                                $"Subtotal vuelos: ${flightTotal:F2}"
                            );

                        column.Item()
                            .Text(
                                $"Equipaje: ${luggageTotal:F2}"
                            );

                        column.Item()
                            .Text(
                                $"TOTAL: ${grandTotal:F2}"
                            )
                            .FontSize(16)
                            .Bold();
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(
                        "Gracias por elegir AirDreams"
                    );
            });
        }).GeneratePdf();
    }

    public byte[] GenerateItinerary(
        ConfirmPurchaseDto dto)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .Text("AIRDREAMS")
                    .FontSize(24)
                    .Bold();

                page.Content()
                    .Column(column =>
                    {
                        column.Spacing(10);

                        column.Item()
                            .Text("ITINERARIO DE VIAJE")
                            .FontSize(18)
                            .Bold();

                        column.Item()
                            .Text(
                                $"Código: {dto.TransactionId}"
                            );

                        column.Item()
                            .Text(
                                $"Clase: {dto.SeatClass}"
                            );

                        column.Item()
                            .Text(
                                $"Cantidad de pasajeros: {dto.PassengerCount}"
                            );

                        column.Item()
                            .PaddingTop(15);

                        column.Item()
                            .Text("VUELOS")
                            .Bold();

                        foreach (var segment in dto.Segments)
                        {
                            column.Item()
                                .Text(
                                    $"Vuelo: {segment.FlightNumber}"
                                );

                            column.Item()
                                .Text(
                                    $"Equipaje registrado: ${segment.CheckedPrice}"
                                );

                            column.Item()
                                .Text(
                                    $"Equipaje de mano: ${segment.CarryOnPrice}"
                                );

                            column.Item()
                                .PaddingBottom(10);
                        }

                        column.Item()
                            .PaddingTop(15);

                        column.Item()
                            .Text("PASAJEROS")
                            .Bold();

                        foreach (var passenger in dto.Passengers)
                        {
                            column.Item()
                                .Text(
                                    $"{passenger.NamePassenger} {passenger.LastnamesPassenger}"
                                );
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(
                        "Presentar este documento durante el viaje"
                    );
            });
        }).GeneratePdf();
    }
}