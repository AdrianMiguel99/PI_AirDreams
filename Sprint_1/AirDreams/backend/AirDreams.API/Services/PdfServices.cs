using AirDreams.API.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class PdfService : IPdfService
{
    private const string Primary = "#032056";
    private const string LightBackground = "#F4F7FB";

    public byte[] GenerateInvoice(ConfirmPurchaseDTO dto)
    {
        var flightTotal = dto.PricePerPassenger * dto.PassengerCount;
        var luggageTotal = CalculateLuggageTotal(dto);
        var grandTotal = flightTotal + luggageTotal;

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Margin(35);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(container =>
                    BuildHeader(container, "FACTURA DE COMPRA", dto.TransactionId)
                );

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Spacing(16);

                    column.Item().Element(container =>
                        BuildInfoCard(container, dto)
                    );

                    column.Item().Text("Pasajeros").FontSize(14).Bold().FontColor(Primary);
                    column.Item().Element(container =>
                        BuildPassengersTable(container, dto)
                    );

                    column.Item().Text("Equipaje").FontSize(14).Bold().FontColor(Primary);
                    column.Item().Element(container =>
                        BuildLuggageTable(container, dto)
                    );

                    column.Item().AlignRight().Width(230).Element(container =>
                        BuildTotals(container, flightTotal, luggageTotal, grandTotal)
                    );
                });

                page.Footer().AlignCenter().Text("Gracias por elegir AirDreams").FontColor(Colors.Grey.Darken1);
            });
        }).GeneratePdf();
    }

    public byte[] GenerateItinerary(ConfirmPurchaseDTO dto)
    {
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Margin(35);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(container =>
                    BuildHeader(container, "ITINERARIO DE VIAJE", dto.TransactionId)
                );

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Spacing(18);

                    column.Item().Element(container =>
                        BuildTripSummary(container, dto)
                    );

                    column.Item().Text("Vuelos").FontSize(14).Bold().FontColor(Primary);

                    foreach (var segment in dto.Segments)
                    {
                        column.Item().Element(container =>
                            BuildFlightCard(container, segment)
                        );
                    }

                    column.Item().Text("Pasajeros").FontSize(14).Bold().FontColor(Primary);
                    column.Item().Element(container =>
                        BuildPassengersTable(container, dto)
                    );

                    column.Item()
                        .Background("#FFF8E1")
                        .Border(1)
                        .BorderColor("#F2C94C")
                        .Padding(10)
                        .Text("Recomendación: presentarse al aeropuerto con al menos 3 horas de anticipación.")
                        .FontSize(10);
                });

                page.Footer().AlignCenter().Text("Presentar este documento durante el viaje").FontColor(Colors.Grey.Darken1);
            });
        }).GeneratePdf();
    }

    private void BuildHeader(IContainer container, string title, string transactionId)
    {
        container
            .Background(Primary)
            .Padding(18)
            .Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("AIRDREAMS")
                        .FontSize(24)
                        .Bold()
                        .FontColor(Colors.White);

                    column.Item().Text(title)
                        .FontSize(13)
                        .FontColor(Colors.White);
                });

                row.ConstantItem(170).AlignRight().Column(column =>
                {
                    column.Item().Text("Código de compra")
                        .FontSize(9)
                        .FontColor(Colors.White);

                    column.Item().Text(transactionId)
                        .FontSize(12)
                        .Bold()
                        .FontColor(Colors.White);
                });
            });
    }

    private void BuildInfoCard(IContainer container, ConfirmPurchaseDTO dto)
    {
        container
            .Background(LightBackground)
            .Border(1)
            .BorderColor("#D8E0EE")
            .Padding(14)
            .Column(column =>
            {
                column.Spacing(6);

                column.Item().Text("Información de compra")
                    .FontSize(13)
                    .Bold()
                    .FontColor(Primary);

                column.Item().Text($"Comprador: {dto.BuyerName}");
                column.Item().Text($"Método de pago: {FormatPaymentMethod(dto.PaymentMethod)}");
                column.Item().Text($"Clase: {FormatSeatClass(dto.SeatClass)}");
                column.Item().Text($"Cantidad de pasajeros: {dto.PassengerCount}");
                column.Item().Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}");
            });
    }

    private void BuildTripSummary(IContainer container, ConfirmPurchaseDTO dto)
    {
        var firstSegment = dto.Segments.FirstOrDefault();
        var lastSegment = dto.Segments.LastOrDefault();

        var origin = firstSegment?.DepartureAirport?.Code ?? "Origen";
        var destination = lastSegment?.ArrivalAirport?.Code ?? "Destino";

        container
            .Background(LightBackground)
            .Border(1)
            .BorderColor("#D8E0EE")
            .Padding(16)
            .Column(column =>
            {
                column.Spacing(8);

                column.Item().Text($"{origin}  →  {destination}")
                    .FontSize(22)
                    .Bold()
                    .FontColor(Primary);

                column.Item().Text($"Clase: {FormatSeatClass(dto.SeatClass)}");
                column.Item().Text($"Pasajeros: {dto.PassengerCount}");
                column.Item().Text($"Código de reserva: {dto.TransactionId}");
            });
    }

    private void BuildFlightCard(IContainer container, FlightSegmentDTO segment)
    {
        var originCode = Safe(segment.DepartureAirport?.Code, "Origen");
        var destinationCode = Safe(segment.ArrivalAirport?.Code, "Destino");

        var originName = Safe(segment.DepartureAirport?.Name, "Aeropuerto de salida");
        var destinationName = Safe(segment.ArrivalAirport?.Name, "Aeropuerto de llegada");

        container
            .Border(1)
            .BorderColor("#D8E0EE")
            .Padding(14)
            .Column(column =>
            {
                column.Spacing(10);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Vuelo {segment.FlightNumber}")
                        .FontSize(14)
                        .Bold()
                        .FontColor(Primary);

                    row.ConstantItem(120).AlignRight()
                    .Text(segment.Duration.ToString(@"hh\:mm"))
                    .FontSize(10)
                    .FontColor(Colors.Grey.Darken2);
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(originCode).FontSize(20).Bold().FontColor(Primary);
                        c.Item().Text(originName).FontSize(9);
                        c.Item().Text(segment.DepartureAirport?.City ?? "").FontSize(9).FontColor(Colors.Grey.Darken1);
                    });

                    row.ConstantItem(60).AlignCenter().Text("→").FontSize(22).Bold().FontColor(Primary);

                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().Text(destinationCode).FontSize(20).Bold().FontColor(Primary);
                        c.Item().Text(destinationName).FontSize(9);
                        c.Item().Text(segment.ArrivalAirport?.City ?? "").FontSize(9).FontColor(Colors.Grey.Darken1);
                    });
                });

                column.Item().LineHorizontal(1).LineColor("#D8E0EE");

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Salida: {FormatDate(segment.DepartureDate)} {segment.DepartureTime}");
                    row.RelativeItem().AlignRight().Text($"Llegada: {FormatDate(segment.ArrivalDate)} {segment.ArrivalTime}");
                });
            });
    }

    private void BuildPassengersTable(IContainer container, ConfirmPurchaseDTO dto)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2);
                columns.RelativeColumn(2);
                columns.RelativeColumn(2);
            });

            table.Header(header =>
            {
                HeaderCell(header.Cell(), "Nombre");
                HeaderCell(header.Cell(), "Correo");
                HeaderCell(header.Cell(), "País");
            });

            foreach (var passenger in dto.Passengers)
            {
                BodyCell(table.Cell(), $"{passenger.NamePassenger} {passenger.LastnamesPassenger}");
                BodyCell(table.Cell(), passenger.EmailPassenger ?? "N/D");
                BodyCell(table.Cell(), passenger.Country);
            }
        });
    }

    private void BuildLuggageTable(IContainer container, ConfirmPurchaseDTO dto)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2);
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                HeaderCell(header.Cell(), "Tipo");
                HeaderCell(header.Cell(), "Cantidad");
                HeaderCell(header.Cell(), "Subtotal");
            });

            if (dto.Luggage == null || !dto.Luggage.Any())
            {
                BodyCell(table.Cell(), "Sin equipaje adicional");
                BodyCell(table.Cell(), "-");
                BodyCell(table.Cell(), "$0.00");
                return;
            }

            foreach (var luggage in dto.Luggage)
            {
                foreach (var item in luggage.LuggageItems)
                {
                    var subtotal = GetLuggageSubtotal(dto, item);

                    BodyCell(table.Cell(), FormatLuggageType(item.Type));
                    BodyCell(table.Cell(), item.Quantity.ToString());
                    BodyCell(table.Cell(), FormatMoney(item.Subtotal));
                }
            }
        });
    }

    private void BuildTotals(IContainer container, decimal flightTotal, decimal luggageTotal, decimal grandTotal)
    {
        container
            .Background(LightBackground)
            .Border(1)
            .BorderColor("#D8E0EE")
            .Padding(12)
            .Column(column =>
            {
                column.Spacing(6);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("Subtotal vuelos");
                    row.ConstantItem(90).AlignRight().Text(FormatMoney(flightTotal));
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("Equipaje");
                    row.ConstantItem(90).AlignRight().Text(FormatMoney(luggageTotal));
                });

                column.Item().LineHorizontal(1).LineColor("#B8C3D6");

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("TOTAL").FontSize(14).Bold().FontColor(Primary);
                    row.ConstantItem(90).AlignRight().Text(FormatMoney(grandTotal)).FontSize(14).Bold().FontColor(Primary);
                });
            });
    }

    private static void HeaderCell(IContainer container, string text)
    {
        container
            .Background(Primary)
            .Padding(6)
            .Text(text)
            .FontColor(Colors.White)
            .Bold();
    }

    private static void BodyCell(IContainer container, string text)
    {
        container
            .BorderBottom(1)
            .BorderColor("#E1E7F0")
            .Padding(6)
            .Text(text);
    }

    private decimal CalculateLuggageTotal(ConfirmPurchaseDTO dto)
    {
        if (dto.Luggage == null)
            return 0;

        return dto.Luggage
            .SelectMany(luggage => luggage.LuggageItems)
            .Sum(item => item.Subtotal);
    }

    private decimal GetLuggageSubtotal(ConfirmPurchaseDTO dto, LuggageItemDTO item)
    {
        if (item.Subtotal > 0)
            return item.Subtotal;

        var unitPrice = item.UnitPrice;

        if (unitPrice <= 0)
        {
            unitPrice = item.Type == "checked"
                ? dto.Segments.Sum(segment => segment.CheckedPrice)
                : dto.Segments.Sum(segment => segment.CarryOnPrice);
        }

        return unitPrice * item.Quantity;
    }

    private static string FormatDate(DateTime? date)
    {
        return date.HasValue
            ? date.Value.ToString("dd/MM/yyyy")
            : "Fecha N/D";
    }

    private static string FormatMoney(decimal value)
    {
        return $"${value:F2}";
    }

    private static string FormatSeatClass(string value)
    {
        return value == "FirstClass"
            ? "Primera clase"
            : "Turista";
    }

    private static string FormatPaymentMethod(string value)
    {
        return value switch
        {
            "Card" => "Tarjeta",
            "PayPal" => "PayPal",
            "GooglePay" => "Google Pay",
            "ApplePay" => "Apple Pay",
            _ => value
        };
    }

    private static string FormatLuggageType(string value)
    {
        return value switch
        {
            "checked" => "Equipaje documentado",
            "carryOn" => "Carry on",
            _ => value
        };
    }

    private static string Safe(string? value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value)
            ? fallback
            : value;
    }
}