using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services;
using Moq;
using NUnit.Framework;

namespace AirDreamsUsersTests;

[TestFixture]
public class CancelReservationSuccessTests
{
    [Test]
    public async Task CancelReservationAsync_WhenReservationExists_ShouldCancelReservation()
    {
        var repositoryMock = new Mock<ICancellationRepository>();
        var emailServiceMock = new Mock<IEmailService>();

        var service = new CancellationService(
            repositoryMock.Object,
            emailServiceMock.Object
        );

        var transactionId = "TXN-TEST";

        repositoryMock
            .Setup(r => r.ItineraryExistsAsync(transactionId))
            .ReturnsAsync(true);

        repositoryMock
            .Setup(r => r.IsItineraryCancelledAsync(transactionId))
            .ReturnsAsync(false);

        var result = await service.CancelReservationAsync(transactionId);

        Assert.That(result, Is.True);

        repositoryMock.Verify(
            r => r.CancelItineraryAsync(transactionId),
            Times.Once
        );
    }
}