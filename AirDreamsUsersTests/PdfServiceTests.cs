using AirDreams.API.DTOs;
using AirDreams.API.Services;
using NUnit.Framework;
using QuestPDF.Infrastructure;

namespace AirDreams.API.Tests;

[TestFixture]
public class PdfServiceTests
{
    [Test]
    public void PdfService_WhenPurchaseIsValid_ShouldGenerateInvoiceAndItineraryPdfs()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var service = new PdfService();

        var dto = new ConfirmPurchaseDTO
        {
            TransactionId = "TXN-TEST",
            BuyerName = "Alexa Alpizar",
            PaymentMethod = "PayPal",
            SeatClass = "FirstClass",
            PricePerPassenger = 60,
            PassengerCount = 1,
            Passengers = new List<PassengerDTO>
            {
                new PassengerDTO
                {
                    NamePassenger = "Alexa",
                    LastnamesPassenger = "Alpizar",
                    EmailPassenger = "alexa@test.com",
                    Country = "CRC"
                }
            },
            Segments = new List<FlightSegmentDTO>
            {
                new FlightSegmentDTO
                {
                    FlightNumber = "AD13220260610",
                    RouteId = 13,
                    DepartureDate = new DateTime(2026, 6, 10),
                    ArrivalDate = new DateTime(2026, 6, 10),
                    DepartureTime = "01:30:00",
                    ArrivalTime = "08:30:00",
                    Duration = new TimeSpan(7, 0, 0),
                    CheckedPrice = 60,
                    CarryOnPrice = 30,
                    Multiplier = 0.2m,
                    DepartureAirport = new AirportDTO
                    {
                        Name = "AEROPUERTO COSTA RICA",
                        Code = "SJO",
                        City = "San José"
                    },
                    ArrivalAirport = new AirportDTO
                    {
                        Name = "AEROPUERTO ARGENTINA",
                        Code = "ARG",
                        City = "Buenos Aires"
                    }
                }
            },
            Luggage = new List<LuggagePerPassengerDto>
            {
                new LuggagePerPassengerDto
                {
                    PassengerIndex = 1,
                    LuggageItems = new List<LuggageItemDTO>
                    {
                        new LuggageItemDTO
                        {
                            Type = "checked",
                            Quantity = 1,
                            UnitPrice = 60,
                            Subtotal = 60
                        },
                        new LuggageItemDTO
                        {
                            Type = "carryOn",
                            Quantity = 1,
                            UnitPrice = 30,
                            Subtotal = 30
                        }
                    }
                }
            }
        };

        var invoicePdf = service.GenerateInvoice(dto);
        var itineraryPdf = service.GenerateItinerary(dto);

        Assert.That(invoicePdf, Is.Not.Null);
        Assert.That(invoicePdf.Length, Is.GreaterThan(0));

        Assert.That(itineraryPdf, Is.Not.Null);
        Assert.That(itineraryPdf.Length, Is.GreaterThan(0));
    }
}